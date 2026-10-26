using Microsoft.EntityFrameworkCore;
using TechTrend.Api.Data;
using TechTrend.Api.Models;

namespace TechTrend.Api.Services
{
    public class UpdateWorker(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<UpdateWorker> logger) : BackgroundService
    {
        private async Task UpdateAllAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var collector = scope.ServiceProvider.GetRequiredService<DataCollector>();
            var calculator = scope.ServiceProvider.GetRequiredService<TrendCalculator>();

            var technologies = await db.Technologies.ToListAsync();

            foreach (var technology in technologies)
            {
                try
                {
                    var activity = await collector.GetActivityAsync(technology.Language, stoppingToken);

                    var previous = await db.TrendSnapshots.
                        Where(s => s.TechnologyId == technology.Id).
                        OrderByDescending(s => s.CollectedAt).
                        FirstOrDefaultAsync(stoppingToken);

                    var change = previous == null ? 0 : calculator.CalculateChange(previous.ActivityValue, activity);

                    var snapshot = new TrendSnapshot
                    {
                        TechnologyId = technology.Id,
                        CollectedAt = DateTime.UtcNow,
                        ActivityValue = activity,
                        Score = previous == null ? 50 : calculator.CalculateScore(change)
                    };

                    db.TrendSnapshots.Add(snapshot);
                    await db.SaveChangesAsync(stoppingToken);

                    // To avoid API overload with cosecutive requests
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch(Exception ex)
                {
                    logger.LogWarning(ex, "Could not update {Technology}", technology.Name);
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalHours = configuration.GetValue<double>("UpdateIntervalHours", 24);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateAllAsync(stoppingToken);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Technology update failed");
                }

                await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
            }
        }
    }
}
