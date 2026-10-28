using Microsoft.EntityFrameworkCore;
using TechTrend.Api.Data;
using TechTrend.Api.Models;

namespace TechTrend.Api.Services
{
    public class TechnologyService
    {
        private readonly AppDbContext _db;

        public TechnologyService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<object> GetTechnologiesAsync(string? search, string? category, string? sort, string? period, string? source)
        {
            period = NormalizePeriod(period);
            source = NormalizeSource(source);

            var query = _db.Technologies.Include(t => t.Snapshots).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Name.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(category) && !string.Equals(category, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Category == category);
            }

            var technologies = await query.ToListAsync();

            var result = technologies.Select(t =>
            {
                var snapshots = t.Snapshots.Where(s => s.Source == source && s.Period == period).OrderByDescending(s => s.CollectedAt).ToList();
                var latest = snapshots.FirstOrDefault();
                var change = CalculateTrend(snapshots.OrderBy(s => s.CollectedAt).ToList(), period);

                return new
                {
                    t.Id,
                    t.Name,
                    t.Category,
                    t.Language,
                    Source = source,
                    Period = period,
                    ActivityValue = latest?.ActivityValue,
                    ChangePercent = change,
                    CollectedAt = latest?.CollectedAt
                };
            });

            result = sort?.ToLowerInvariant() switch
            {
                "highest" => result.OrderByDescending(t => t.ChangePercent ?? double.NegativeInfinity),

                "lowest" => result.OrderBy(t => t.ChangePercent ?? double.PositiveInfinity),

                "score" => result.OrderByDescending(t => t.ActivityValue ?? -1),

                _ => result.OrderBy(t => t.Name)
            };

            return result.ToList();
        }

        public async Task<object?> GetTechnologyByIdAsync(int id, string? period = "24h", string? source = "GitHub")
        {
            period = NormalizePeriod(period);
            source = NormalizeSource(source);

            var technology = await _db.Technologies.Include(t => t.Snapshots).FirstOrDefaultAsync(t => t.Id == id);

            if (technology == null) 
            {
                return null;
            }

            var snapshots = technology.Snapshots.Where(s => s.Source == source && s.Period == period).OrderBy(s => s.CollectedAt).ToList();
            var latest = snapshots.LastOrDefault();
            double? trendPercent = CalculateTrend(snapshots, period);

            var cutoff = period switch
            {
                "24h" => DateTime.UtcNow.AddHours(-24),
                "7d" => DateTime.UtcNow.AddDays(-7),
                _ => DateTime.UtcNow.AddDays(-30)
            };

            var history = snapshots.Where(s => s.CollectedAt >= cutoff).Select(s => new
                {
                    collectedAt = s.CollectedAt,
                    activityValue = s.ActivityValue
                }).ToList();

            var recommendation = trendPercent switch
            {
                null => "Not enough data to determine the current trend.",
                > 0 => "Activity is increasing. Monitor this technology.",
                < 0 => "Activity is decreasing. Monitor further changes.",
                _ => "Activity is stable."
            };

            return new
            {
                id = technology.Id,
                name = technology.Name,
                source,
                period,
                activityValue = latest?.ActivityValue,
                trendPercent,
                recommendation,
                history
            };
        }

        private static double? CalculateTrend(List<TrendSnapshot> snapshots, string period)
        {
            if (snapshots.Count < 2)
            {
                return null;
            }

            var latest = snapshots[^1];

            var cutoff = period switch
            {
                "24h" => latest.CollectedAt.AddHours(-24),
                "7d" => latest.CollectedAt.AddDays(-7),
                "30d" => latest.CollectedAt.AddDays(-30),
                _ => latest.CollectedAt.AddHours(-24)
            };

            var previous = snapshots.Where(s => s.CollectedAt >= cutoff && s.CollectedAt < latest.CollectedAt).OrderBy(s => s.CollectedAt).FirstOrDefault();
            if (previous == null || previous.ActivityValue <= 0)
            {
                return null;
            }

            return Math.Round((latest.ActivityValue - previous.ActivityValue) / (double)previous.ActivityValue * 100, 2);
        }

        private static string NormalizePeriod(string? period)
        {
            return period?.ToLowerInvariant() switch
            {
                "7d" => "7d",
                "30d" => "30d",
                _ => "24h"
            };
        }

        private static string NormalizeSource(string? source)
        {
            return source?.ToLowerInvariant() switch
            {
                "stackoverflow" => "StackOverflow",
                "himalayas" => "Himalayas",
                _ => "GitHub"
            };
        }
    }
}