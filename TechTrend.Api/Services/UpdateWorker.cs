
using Microsoft.EntityFrameworkCore;
using TechTrend.Api.Data;
using TechTrend.Api.Models;

namespace TechTrend.Api.Services
{
    public class UpdateWorker(IServiceScopeFactory scopeFactory, ILogger<UpdateWorker> logger) : BackgroundService
    {
        private async Task UpdateAllAsync(CancellationToken stoppingToken, bool includeDailyPeriods)
        {
            using var scope = scopeFactory.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var collector = scope.ServiceProvider.GetRequiredService<DataCollector>();
            var calculator = scope.ServiceProvider.GetRequiredService<TrendCalculator>();

            var technologies = await db.Technologies.ToListAsync(stoppingToken);
            foreach (var technology in technologies)
            {
                var periods = includeDailyPeriods ? new[] { "24h", "7d", "30d" } : new[] { "24h" };

                foreach (var period in periods)
                {
                    await TryUpdateAsync(db, calculator, technology, "GitHub", period, () => collector.GetActivityAsync(technology.Language, period, stoppingToken), stoppingToken);
                    await TryUpdateAsync(db, calculator, technology, "StackOverflow", period, () => collector.GetStackOverflowActivityAsync(technology.Language, period, stoppingToken), stoppingToken);
                }
                await TryUpdateAsync(db, calculator, technology, "Himalayas", "24h", () => collector.GetHimalayasJobsAsync(technology.Name, stoppingToken), stoppingToken);
            }
        }

        private async Task TryUpdateAsync(AppDbContext db, TrendCalculator calculator, Technology technology, string source, string period, Func<Task<int>> collectActivity, CancellationToken stoppingToken)
        {
            try
            {
                var activity = await collectActivity();
                var previous = await db.TrendSnapshots.Where(s => s.TechnologyId == technology.Id && s.Source == source && s.Period == period).OrderByDescending(s => s.CollectedAt).FirstOrDefaultAsync(stoppingToken);
                var change = previous == null ? 0 : calculator.CalculateChange(previous.ActivityValue, activity);

                db.TrendSnapshots.Add(new TrendSnapshot
                {
                    TechnologyId = technology.Id,
                    Source = source,
                    Period = period,
                    CollectedAt = DateTime.UtcNow,
                    ActivityValue = activity,
                    Score = previous == null ? 50 : calculator.CalculateScore(change)
                });

                await db.SaveChangesAsync(stoppingToken);
                logger.LogInformation("Updated {Technology} from {Source} ({Period}): {Activity}", technology.Name, source, period, activity);

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not update {Technology} from {Source} ({Period})", technology.Name, source, period);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastDailyUpdate = DateTime.MinValue;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var today = DateTime.UtcNow.Date;
                    var includeDailyPeriods = lastDailyUpdate != today;

                    await UpdateAllAsync(stoppingToken, includeDailyPeriods);

                    if (includeDailyPeriods)
                    {
                        lastDailyUpdate = today;
                    }
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Technology update failed");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
