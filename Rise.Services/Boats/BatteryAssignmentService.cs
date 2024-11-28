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

        private async Task HandleBatteryAssignments()
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            
            // Get active reservations first
            var activeReservations = await _dbContext.Reservations
                .Include(r => r.Battery)
                .Include(r => r.User)
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.Battery != null && 
                    r.TimeSlot.Date == today &&
                    r.TimeSlot.Start <= TimeOnly.FromDateTime(now) &&
                    r.TimeSlot.End > TimeOnly.FromDateTime(now))
                .ToListAsync();

            // Get all batteries with their current holders
            var allBatteries = await _dbContext.Batteries
                .Include(b => b.CurrentHolder)
                .ToListAsync();

            // Clear holders only for batteries not in active reservations
            foreach (var battery in allBatteries)
            {
                if (!activeReservations.Any(r => r.Battery == battery))
                {
                    battery.AssignToHolder(null);
                }
            }

            // Assign holders for active reservations
            foreach (var reservation in activeReservations)
            {
                if (reservation.Battery != null)
                {
                    reservation.Battery.AssignToHolder(reservation.User);
                    _logger.LogInformation(
                        "Assigned battery {BatteryId} to user {UserId} for active reservation",
                        reservation.Battery.Id,
                        reservation.UserId);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AssignBatteriesToUpcomingReservations()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var twoDaysFromNow = today.AddDays(2);
            
            var reservationsNeedingBatteries = await _dbContext.Reservations
                .Include(r => r.Boat)
                .ThenInclude(b => b.Batteries)
                .Include(r => r.TimeSlot)
                .Where(r => 
                    (r.TimeSlot.Date == today || r.TimeSlot.Date == twoDaysFromNow) && 
                    r.Battery == null)
                .OrderBy(r => r.TimeSlot.Start)
                .ToListAsync();

            _logger.LogInformation(
                "Found {Count} reservations needing battery assignment for today and {Date}", 
                reservationsNeedingBatteries.Count, 
                twoDaysFromNow);

            var reservationsByBoat = reservationsNeedingBatteries.GroupBy(r => r.BoatId);
            
            foreach (var boatGroup in reservationsByBoat)
            {
                var reservations = boatGroup.OrderBy(r => r.TimeSlot.Start).ToList();
                
                foreach (var reservation in reservations)
                {
                    try
                    {
                        var availableBattery = reservation.Boat.GetAvailableBatteryForDate(
                            reservation.TimeSlot.Date,
                            reservation.TimeSlot.Start,
                            reservation.TimeSlot.End
                        );

                        if (availableBattery != null)
                        {
                            reservation.Battery = availableBattery;
                            
                            _logger.LogInformation(
                                "Assigned battery {BatteryId} to reservation {ReservationId} for boat {BoatId} at {StartTime}-{EndTime}",
                                availableBattery.Id,
                                reservation.Id,
                                reservation.BoatId,
                                reservation.TimeSlot.Start,
                                reservation.TimeSlot.End);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "No available battery found for reservation {ReservationId} on boat {BoatId} at {StartTime}-{EndTime}",
                                reservation.Id,
                                reservation.BoatId,
                                reservation.TimeSlot.Start,
                                reservation.TimeSlot.End);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error assigning battery to reservation {ReservationId}",
                            reservation.Id);
                    }
                }
            }

            await HandleBatteryAssignments();
        }

        public async Task OptimizeBatteryAssignments()
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var twoDaysFromNow = today.AddDays(2);
            
            // First, get all reservations that need batteries
            var reservations = await _dbContext.Reservations
                .Include(r => r.Battery)
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.TimeSlot.Date >= today && 
                    r.TimeSlot.Date <= twoDaysFromNow &&
                    (r.TimeSlot.Date > today || 
                     (r.TimeSlot.Date == today && 
                      r.TimeSlot.End > TimeOnly.FromDateTime(now)))
                )
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .ToListAsync();

            // Get all batteries directly from the Battery table
            var allBatteries = await _dbContext.Batteries
                .Include(b => b.CurrentHolder)
                .OrderBy(b => b.UsageCount)
                .ToListAsync();

            // Clear all existing battery assignments first
            foreach (var reservation in reservations)
            {
                if (reservation.Battery != null)
                {
                    _logger.LogInformation(
                        "Clearing battery {BatteryId} from reservation {ReservationId}",
                        reservation.Battery.Id,
                        reservation.Id);
                    reservation.Battery = null;
                }
            }

            // Group reservations by date
            var reservationsByDate = reservations.GroupBy(r => r.TimeSlot.Date);
            
            foreach (var dateGroup in reservationsByDate)
            {
                var dateReservations = dateGroup.OrderBy(r => r.TimeSlot.Start).ToList();
                var usedBatteriesForDate = new HashSet<int>();

                foreach (var reservation in dateReservations)
                {
                    // Find an available battery that belongs to the boat and hasn't been used today
                    var availableBattery = allBatteries
                        .Where(b => 
                            b.BoatId == reservation.BoatId && 
                            !usedBatteriesForDate.Contains(b.Id))
                        .OrderBy(b => b.Reservations.Count(r => 
                            r.TimeSlot.Date >= today && 
                            r.TimeSlot.Date <= twoDaysFromNow))
                        .ThenBy(b => b.UsageCount)
                        .FirstOrDefault();

                    if (availableBattery != null)
                    {
                        reservation.Battery = availableBattery;
                        usedBatteriesForDate.Add(availableBattery.Id);
                        
                        _logger.LogInformation(
                            "Assigned battery {BatteryId} to reservation {ReservationId} for boat {BoatId} at {Date} {StartTime}-{EndTime}",
                            availableBattery.Id,
                            reservation.Id,
                            reservation.BoatId,
                            reservation.TimeSlot.Date,
                            reservation.TimeSlot.Start,
                            reservation.TimeSlot.End);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "No available battery found for reservation {ReservationId} on boat {BoatId} at {Date} {StartTime}-{EndTime}",
                            reservation.Id,
                            reservation.BoatId,
                            reservation.TimeSlot.Date,
                            reservation.TimeSlot.Start,
                            reservation.TimeSlot.End);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            await HandleBatteryAssignments();
        }

        public async Task ResetAllBatteryAssignments()
        {
            _logger.LogInformation("Resetting all battery assignments");
            
            // Get all batteries
            var allBatteries = await _dbContext.Batteries
                .Include(b => b.CurrentHolder)
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
