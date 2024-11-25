using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rise.Services.Boats;

namespace Rise.Server.Workers
{
    public class BatteryAssignmentWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BatteryAssignmentWorker> _logger;

        public BatteryAssignmentWorker(
            IServiceProvider services,
            ILogger<BatteryAssignmentWorker> logger)
        {
            _services = services;
            _logger = logger;
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
                    
                    // Run once per hour
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while assigning batteries");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
