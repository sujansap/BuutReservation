using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Persistence;
using Microsoft.Extensions.Logging;

namespace Rise.Services.Boats
{
    public class BatteryAssignmentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<BatteryAssignmentService> _logger;

        public BatteryAssignmentService(
            ApplicationDbContext dbContext,
            ILogger<BatteryAssignmentService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AssignAndOptimizeBatteries()
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var threeDaysFromNow = today.AddDays(3);
            var currentTime = TimeOnly.FromDateTime(now);

            // First handle unassigning batteries from completed reservations
            var completedReservations = await _dbContext.Reservations
                .Include(r => r.Battery)
                .Include(r => r.User)
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.Battery != null && 
                    (r.TimeSlot.Date < today || 
                     (r.TimeSlot.Date == today && r.TimeSlot.End <= currentTime)))
                .ToListAsync();

            foreach (var reservation in completedReservations)
            {
                var lastUser = reservation.User;
                var battery = reservation.Battery!;
                reservation.Battery = null;
                battery.AssignToHolder(lastUser);
            }

            // Get all upcoming reservations, including those with batteries
            var upcomingReservations = await _dbContext.Reservations
                .Include(r => r.Boat)
                .ThenInclude(b => b.Batteries)
                .Include(r => r.TimeSlot)
                .Include(r => r.User)
                .Include(r => r.Battery)
                .Where(r => 
                    r.TimeSlot.Date >= today && 
                    r.TimeSlot.Date <= threeDaysFromNow &&
                    (r.TimeSlot.Date > today || 
                     (r.TimeSlot.Date == today && r.TimeSlot.Start > currentTime)))
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .ToListAsync();

            // Get all batteries with their usage info
            var allBatteries = await _dbContext.Batteries
                .Include(b => b.Reservations)
                .Include(b => b.CurrentHolder)
                .OrderBy(b => b.UsageCount)
                .ToListAsync();

            // Group reservations by date for priority assignment
            var reservationsByDate = upcomingReservations
                .GroupBy(r => r.TimeSlot.Date)
                .OrderBy(g => g.Key);

            foreach (var dateGroup in reservationsByDate)
            {
                foreach (var reservation in dateGroup.OrderBy(r => r.TimeSlot.Start))
                {
                    if (reservation.Battery == null)
                    {
                        // Get compatible batteries for this boat
                        var compatibleBatteries = allBatteries
                            .Where(b => b.BoatId == reservation.BoatId)
                            .OrderBy(b => b.UsageCount)
                            .ToList();

                        // Find an available battery
                        var availableBattery = compatibleBatteries
                            .FirstOrDefault(b => b.IsAvailableFor(
                                reservation.TimeSlot.Date,
                                reservation.TimeSlot.Start,
                                reservation.TimeSlot.End,
                                now));

                        if (availableBattery != null)
                        {
                            reservation.Battery = availableBattery;
                        }
                    }

                    // Always update the current holder if there's a battery assigned
                    if (reservation.Battery != null)
                    {
                        reservation.Battery.AssignToHolder(reservation.User);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task ResetAllBatteryAssignments()
        {
            _logger.LogInformation("Resetting all battery assignments");
            
            // Get all batteries
            var allBatteries = await _dbContext.Batteries
                .Include(b => b.CurrentHolder)
                .Include(b => b.Mentor)
                .ToListAsync();

            // Reset all current holders
            foreach (var battery in allBatteries)
            {
                battery.AssignToHolder(null);
            }

            // Clear all battery assignments for today and tomorrow's reservations
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var tomorrow = today.AddDays(1);
            
            var reservationsToReset = await _dbContext.Reservations
                .Include(r => r.Battery)
                .Where(r => 
                    r.Battery != null && 
                    (r.TimeSlot.Date == today || r.TimeSlot.Date == tomorrow))
                .ToListAsync();

            foreach (var reservation in reservationsToReset)
            {
                reservation.Battery = null;
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully reset all battery assignments");
        }
    }
}
