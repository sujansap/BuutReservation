using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Persistence;

namespace Rise.Services.Boats
{
    public class BatteryAssignmentService(ApplicationDbContext dbContext)
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task AssignBatteriesToUpcomingReservations()
        {
            // Get reservations that need batteries assigned
            var upcomingReservations = await GetUpcomingReservationsNeedingBatteries();
            
            foreach (var reservation in upcomingReservations)
            {
                // Get least used battery for the boat
                var battery = await GetLeastUsedBatteryForBoat(reservation.BoatId) ?? throw new InvalidOperationException($"No available batteries found for boat {reservation.BoatId}");
                await AssignBatteryToReservation(reservation, battery);
            }
        }

        private async Task<List<Reservation>> GetUpcomingReservationsNeedingBatteries()
        {
            var twoDaysFromNow = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.TimeSlot.Date <= twoDaysFromNow && 
                    r.TimeSlot.Date >= today &&
                    EF.Property<int?>(r, "BatteryId") == null)
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .ToListAsync();
        }

        private async Task<Battery?> GetLeastUsedBatteryForBoat(int boatId)
        {
            var date = DateOnly.FromDateTime(DateTime.Now);
            return await _dbContext.Batteries
                .Where(b => b.BoatId == boatId)
                .Where(b => !b.Reservations.Any(r => 
                    r.TimeSlot.Date == date || 
                    r.TimeSlot.Date == date.AddDays(1)))
                .OrderBy(b => b.Reservations.Count)
                .FirstOrDefaultAsync();
        }

        private async Task AssignBatteryToReservation(Reservation reservation, Battery battery)
        {
            reservation.Battery = battery;
            await _dbContext.SaveChangesAsync();
        }
    }
}
