using Rise.Services.Boats;

namespace Rise.Server.Workers;

public class BatteryAssignmentWorker : BackgroundService
{
    private readonly ILogger<BatteryAssignmentWorker> _logger;
    private readonly IServiceProvider _services;
    private const int CheckIntervalMinutes = 15;

    public BatteryAssignmentWorker(
        ILogger<BatteryAssignmentWorker> logger,
        IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var batteryService = scope.ServiceProvider.GetRequiredService<BatteryAssignmentService>();
                await batteryService.AssignBatteriesToUpcomingReservations();
                
                _logger.LogInformation("Battery assignment check completed at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning batteries");
            }

            await Task.Delay(TimeSpan.FromMinutes(CheckIntervalMinutes), stoppingToken);
        }
    }
}