using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Data;

namespace Thakkirni.API.Services
{
    /// <summary>
    /// Background service that runs every 5 minutes and marks any non-completed
    /// task whose DueDate has passed as OVERDUE.
    /// </summary>
    public class OverdueStatusService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OverdueStatusService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        public OverdueStatusService(IServiceScopeFactory scopeFactory, ILogger<OverdueStatusService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OverdueStatusService started.");

            // Run once immediately on startup, then repeat on the interval.
            while (!stoppingToken.IsCancellationRequested)
            {
                await MarkOverdueItemsAsync();
                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task MarkOverdueItemsAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;

                // Find all items that are still in TODO state but whose DueDate has passed.
                var overdueItems = await context.Items
                    .Where(i => i.Status == "TODO" && i.DueDate < now)
                    .ToListAsync();

                if (overdueItems.Count == 0)
                    return;

                foreach (var item in overdueItems)
                {
                    item.Status = "OVERDUE";
                    item.UpdatedAt = now;
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Marked {Count} item(s) as OVERDUE.", overdueItems.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OverdueStatusService.");
            }
        }
    }
}
