using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Persistence;
using Microsoft.Extensions.Logging;

namespace Rise.Services.Boats
{
    internal record TimeSlotInfo(DateOnly Date, TimeOnly Start, TimeOnly End);

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

        public async Task AssignBatteriesToUpcomingReservations()
        {
            var upcomingReservations = await GetUpcomingReservationsNeedingBatteries();
            _logger.LogInformation($"Found {upcomingReservations.Count} reservations needing batteries");

            foreach (var reservation in upcomingReservations.OrderBy(r => r.TimeSlot.Date).ThenBy(r => r.TimeSlot.Start))
            {
                try
                {
                    _logger.LogInformation($"Processing reservation {reservation.Id} for date {reservation.TimeSlot.Date} at {reservation.TimeSlot.Start}");
                    
                    var boat = await _dbContext.Boats
                        .Include(b => b.Batteries)
                            .ThenInclude(b => b.Reservations)
                                .ThenInclude(r => r.TimeSlot)
                        .FirstOrDefaultAsync(b => b.Id == reservation.BoatId)
                        ?? throw new InvalidOperationException($"Boat {reservation.BoatId} not found");

                    _logger.LogInformation($"Checking {boat.Batteries.Count} batteries for boat {boat.Id}");

                    // Get all existing reservations for this boat's batteries
                    var existingReservations = boat.Batteries
                        .SelectMany(b => b.Reservations)
                        .Where(r => Math.Abs((r.TimeSlot.Date.DayNumber - reservation.TimeSlot.Date.DayNumber)) <= 2)
                        .ToList();

                    _logger.LogInformation($"Found {existingReservations.Count} existing reservations to check against");

                    var availableBattery = boat.Batteries
                        .OrderBy(b => b.Reservations.Count)
                        .FirstOrDefault(b => IsAvailableForReservation(b, reservation, existingReservations));

                    if (availableBattery != null)
                    {
                        _logger.LogInformation($"Assigning battery {availableBattery.Id} to reservation {reservation.Id}");
                        reservation.Battery = availableBattery;
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogWarning($"No available battery found for reservation {reservation.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error assigning battery to reservation {reservation.Id}");
                }
            }
        }

        private bool IsAvailableForReservation(Battery battery, Reservation newReservation, List<Reservation> existingReservations)
        {
            const int ChargingHours = 4;
            const int ReservationHours = 3;

            var newStart = newReservation.TimeSlot.Date.ToDateTime(newReservation.TimeSlot.Start);
            var newEnd = newStart.AddHours(ReservationHours);
            var newEndWithCharging = newEnd.AddHours(ChargingHours);

            _logger.LogInformation($"Checking battery {battery.Id} availability for {newStart} to {newEnd} (charging until {newEndWithCharging})");

            foreach (var existing in existingReservations.Where(r => r.BatteryId == battery.Id))
            {
                var existingStart = existing.TimeSlot.Date.ToDateTime(existing.TimeSlot.Start);
                var existingEnd = existingStart.AddHours(ReservationHours);
                var existingEndWithCharging = existingEnd.AddHours(ChargingHours);

                _logger.LogInformation($"Comparing with existing reservation: {existingStart} to {existingEnd} (charging until {existingEndWithCharging})");

                if (newStart < existingEndWithCharging && newEndWithCharging > existingStart)
                {
                    _logger.LogInformation($"Conflict detected for battery {battery.Id}");
                    return false;
                }
            }

            return true;
        }

        private async Task<List<Reservation>> GetUpcomingReservationsNeedingBatteries()
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = startDate.AddDays(7);

            return await _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.TimeSlot.Date >= startDate && 
                    r.TimeSlot.Date <= endDate &&
                    r.BatteryId == null)
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .ToListAsync();
        }

        public async Task CleanupCompletedBatteryAssignments()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var completedReservations = await _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.TimeSlot.Date < today && 
                    r.BatteryId != null)
                .ToListAsync();

            foreach (var reservation in completedReservations)
            {
                reservation.Battery = null;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<Battery?> GetLeastUsedBatteryForBoat(int boatId)
        {
            var date = DateOnly.FromDateTime(DateTime.Now);
            var batteries = await _dbContext.Batteries
                .Include(b => b.Reservations)
                .ThenInclude(r => r.TimeSlot)
                .Where(b => b.BoatId == boatId)
                .ToListAsync();

            // Get all reservations for today and tomorrow
            var relevantReservations = await _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.BoatId == boatId && 
                    (r.TimeSlot.Date == date || r.TimeSlot.Date == date.AddDays(1)))
                .ToListAsync();

            return batteries
                .Where(battery => !HasTimeConflict(battery, relevantReservations))
                .OrderBy(b => b.Reservations.Count)
                .FirstOrDefault();
        }

        private bool HasTimeConflict(Battery battery, List<Reservation> reservations)
        {
            // Get all time slots where this battery is already assigned
            var batteryTimeSlots = battery.Reservations
                .Where(r => r.TimeSlot.Date == DateOnly.FromDateTime(DateTime.Now) || 
                            r.TimeSlot.Date == DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
                .Select(r => (r.TimeSlot.Date, r.TimeSlot.Start, r.TimeSlot.End))
                .ToList();

            // Check each reservation for time conflicts
            foreach (var reservation in reservations)
            {
                foreach (var usedSlot in batteryTimeSlots)
                {
                    if (reservation.TimeSlot.Date == usedSlot.Date)
                    {
                        // Check if time slots overlap
                        if (!(reservation.TimeSlot.End <= usedSlot.Start || 
                              reservation.TimeSlot.Start >= usedSlot.End))
                        {
                            return true; // There is a conflict
                        }
                    }
                }
            }

            return false;
        }
    }
}
