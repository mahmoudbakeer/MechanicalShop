using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MechanicShop.Infrastructure.BackgroundServices;

public class OverDueOrdersCleanUpService(
    ILogger<OverDueOrdersCleanUpService> logger,
    IServiceScopeFactory serviceScopeFactory,
    TimeProvider timeProvider,
    IOptions<AppSettings> options
) : BackgroundService
{
    private readonly ILogger<OverDueOrdersCleanUpService> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly AppSettings _appSettings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromMinutes(_appSettings.OverdueBookingCleanupFrequencyMinutes)
        );

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    "Starting overdue orders cleanup at {Time}.",
                    _timeProvider.GetUtcNow()
                );
                await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                var cutoff = _timeProvider
                    .GetUtcNow()
                    .AddMinutes(-_appSettings.BookingCancellationThresholdMinutes);
                var overdue = await db
                    .WorkOrders.Where(w =>
                        w.State == WorkOrderState.Scheduled && w.StartedAtUtc <= cutoff
                    )
                    .ToListAsync(stoppingToken);

                if (overdue.Count > 0)
                {
                    _logger.LogInformation("Cleaning up {Count} overdue orders.", overdue.Count);

                    int cancelledCount = 0;
                    foreach (var order in overdue)
                    {
                        var CancellationResult = order.Cancel();

                        if (CancellationResult.IsError)
                        {
                            _logger.LogWarning(
                                "Failed to cancel order {OrderId}: {Errors}",
                                order.Id,
                                CancellationResult.Errors
                            );
                            continue;
                        }
                        cancelledCount++;
                    }
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation(
                        "Successfully cleaned up {count} overdue Orders.",
                        cancelledCount
                    );
                }
                else
                {
                    _logger.LogInformation("No overdue orders found for cleanup.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning up overdue orders.");
            }
        }
    }
}
