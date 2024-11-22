using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Persistence;

namespace Rise.Services.Boats
{
    internal record TimeSlotInfo(DateOnly Date, TimeOnly Start, TimeOnly End);

    public class BatteryAssignmentService(ApplicationDbContext dbContext)
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task AssignBatteriesToUpcomingReservations()
        {
            await UnassignCompletedReservations();

            var upcomingReservations = await GetUpcomingReservationsNeedingBatteries();
            
            foreach (var reservation in upcomingReservations)
            {
                var battery = await GetAvailableBatteryForBoat(
                    reservation.BoatId, 
                    reservation.TimeSlot.Date,
                    reservation.TimeSlot.Start) 
                    ?? throw new InvalidOperationException($"No available batteries found for boat {reservation.BoatId}");
                    
                await AssignBatteryToReservation(reservation, battery);
            }
        }

        private async Task UnassignCompletedReservations()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var completedReservations = await _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Include(r => r.Battery)
                .Where(r => r.TimeSlot.Date < today && r.BatteryId != null)
                .ToListAsync();

            foreach (var reservation in completedReservations)
            {
                reservation.Battery = null;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<List<Reservation>> GetUpcomingReservationsNeedingBatteries()
        {
            var twoDaysFromNow = DateOnly.FromDateTime(DateTime.Now.AddDays(2));

            return await _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.TimeSlot.Date == twoDaysFromNow &&
                    EF.Property<int?>(r, "BatteryId") == null)
                .OrderBy(r => r.TimeSlot.Start)
                .ToListAsync();
        }

        private async Task<Battery?> GetAvailableBatteryForBoat(int boatId, DateOnly date, TimeOnly startTime)
        {
            var boat = await _dbContext.Boats
                .Include(b => b.Batteries)
                    .ThenInclude(b => b.Reservations)
                        .ThenInclude(r => r.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == boatId);

            return boat?.GetAvailableBatteryForDate(date, startTime);
        }

        private async Task AssignBatteryToReservation(Reservation reservation, Battery battery)
        {
            reservation.Battery = battery;
            await _dbContext.SaveChangesAsync();
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
    }
}
