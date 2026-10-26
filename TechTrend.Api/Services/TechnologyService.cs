using Microsoft.EntityFrameworkCore;
using TechTrend.Api.Data;

namespace TechTrend.Api.Services
{
    public class TechnologyService(AppDbContext db)
    {
        public async Task<object> GetTechnologiesAsync(
        string? search,
        string? category,
        string? sort)
        {
            var query = db.Technologies.Include(t => t.Snapshots).AsQueryable();

            // Name filtering
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Name.ToLower().Contains(search.ToLower()));
            }

            // Category filtering
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t => t.Category == category);
            }

            var technologies = await query.ToListAsync();

            // Getting last value for each technology
            var result = technologies.Select(t =>
            {
                var snapshots = t.Snapshots.OrderByDescending(s => s.CollectedAt).ToList();

                var latest = snapshots.FirstOrDefault();
                var previous = snapshots.Skip(1).FirstOrDefault();

                var change = previous == null || previous.ActivityValue == 0
                    ? 0: (latest!.ActivityValue - previous.ActivityValue) / (double)previous.ActivityValue * 100;

                return new
                {
                    t.Id,
                    t.Name,
                    t.Category,
                    t.Language,
                    ActivityValue = latest?.ActivityValue ?? 0,
                    Score = latest?.Score ?? 50,
                    ChangePercent = Math.Round(change, 2)
                };
            });

            // Sorting
            result = sort?.ToLower() switch
            {
                "highest" => result.OrderByDescending(t => t.ChangePercent),
                "lowest" => result.OrderBy(t => t.ChangePercent),
                "score" => result.OrderByDescending(t => t.Score),
                _ => result.OrderBy(t => t.Name)
            };

            return result.ToList();
        }
    }
}
