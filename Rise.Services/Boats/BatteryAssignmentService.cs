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
            
            // Assign batteries for upcoming reservations
            var upcomingReservations = await _dbContext.Reservations
                .Include(r => r.Battery)
                .Include(r => r.User)
                .Where(r => 
                    r.Battery != null && 
                    r.TimeSlot.Date == DateOnly.FromDateTime(now.Date) &&
                    r.TimeSlot.Start <= TimeOnly.FromDateTime(now) &&
                    r.TimeSlot.End > TimeOnly.FromDateTime(now))
                .ToListAsync();

            foreach (var reservation in upcomingReservations)
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
            var twoDaysFromNow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
            
            var reservationsNeedingBatteries = await _dbContext.Reservations
                .Include(r => r.Boat)
                .ThenInclude(b => b.Batteries)
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.TimeSlot.Date == twoDaysFromNow && 
                    r.Battery == null)
                .OrderBy(r => r.TimeSlot.Start)
                .ToListAsync();

            _logger.LogInformation(
                "Found {Count} reservations needing battery assignment for {Date}", 
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
    }
}
