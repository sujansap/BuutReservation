using Rise.Services.Boats;

namespace Rise.Server.Workers;

public class BatteryAssignmentWorker(
    IServiceProvider services,
    ILogger<BatteryAssignmentWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<BatteryAssignmentWorker> _logger = logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try 
        {
            _logger.LogInformation("Battery assignment worker starting initial check at {time}", DateTime.Now);
            await AssignBatteries();
            _logger.LogInformation("Initial battery assignment completed at {time}", DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during initial battery assignment");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_checkInterval, stoppingToken);
                await AssignBatteries();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning batteries to reservations");
            }
        }
    }

    private async Task AssignBatteries()
    {
        using var scope = _services.CreateScope();
        var batteryService = scope.ServiceProvider.GetRequiredService<BatteryAssignmentService>();
        
        await batteryService.CleanupCompletedBatteryAssignments();
        await batteryService.AssignBatteriesToUpcomingReservations();
        
        _logger.LogInformation("Completed battery assignment and cleanup at {time}", DateTime.Now);
    }
}