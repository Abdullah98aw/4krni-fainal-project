// OverdueStatusService is intentionally disabled.
// Task status is now computed dynamically from DueDate and CompletedDate.
// No background mutation of stored status is needed or desired.
// This file is kept as a placeholder to avoid breaking the DI registration in Program.cs.

using Microsoft.Extensions.Hosting;

namespace Thakkirni.API.Services
{
    public class OverdueStatusService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // No-op: status is computed dynamically via Item.ComputedStatus
            return Task.CompletedTask;
        }
    }
}
