using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Persistence;
using Microsoft.Extensions.Logging;
using Rise.Domain.Common;

namespace Rise.Services.Boats
{
    public class BatteryAssignmentService(
        ApplicationDbContext dbContext,
        ILogger<BatteryAssignmentService> logger)
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<BatteryAssignmentService> _logger = logger;

        public async Task AssignAndOptimizeBatteries()
        {
            var timeInfo = GetCurrentTimeInfo();
            
            await HandleCompletedReservations(timeInfo);
            var upcomingReservations = await GetUpcomingReservations(timeInfo);
            var allBatteries = await GetAllBatteriesWithUsageInfo();
            
            await Battery.AssignBatteriesToReservationsAsync(upcomingReservations, allBatteries, timeInfo);
            
            await _dbContext.SaveChangesAsync();
        }

        private static TimeInfo GetCurrentTimeInfo()
        {
            var now = DateTime.UtcNow;
            return new TimeInfo(
                Now: now,
                Today: DateOnly.FromDateTime(now),
                CurrentTime: TimeOnly.FromDateTime(now),
                ThreeDaysFromNow: DateOnly.FromDateTime(now).AddDays(3)
            );
        }

        private async Task HandleCompletedReservations(TimeInfo timeInfo)
        {
            var completedReservations = await _dbContext.Reservations
                .Include(r => r.Battery)
                .Include(r => r.User)
                .Include(r => r.TimeSlot)
                .Where(r => 
                    r.Battery != null && 
                    (r.TimeSlot.Date < timeInfo.Today || 
                     (r.TimeSlot.Date == timeInfo.Today && r.TimeSlot.End <= timeInfo.CurrentTime)))
                .ToListAsync();

            Battery.HandleCompletedReservations(completedReservations);
        }

        private async Task<List<Reservation>> GetUpcomingReservations(TimeInfo timeInfo)
        {
            return await _dbContext.Reservations
                .Include(r => r.Boat)
                .ThenInclude(b => b.Batteries)
                .Include(r => r.TimeSlot)
                .Include(r => r.User)
                .Include(r => r.Battery)
                .Where(r => 
                    r.TimeSlot.Date >= timeInfo.Today && 
                    r.TimeSlot.Date <= timeInfo.ThreeDaysFromNow &&
                    (r.TimeSlot.Date > timeInfo.Today || 
                     (r.TimeSlot.Date == timeInfo.Today && r.TimeSlot.Start > timeInfo.CurrentTime)) &&
                    !r.IsDeleted)
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .ToListAsync();
        }

        private async Task<List<Battery>> GetAllBatteriesWithUsageInfo()
        {
            return await _dbContext.Batteries
                .Include(b => b.Reservations)
                .Include(b => b.CurrentHolder)
                .OrderBy(b => b.UsageCount)
                .ToListAsync();
        }

        public async Task ResetAllBatteryAssignments()
        {
            _logger.LogInformation("Resetting all battery assignments");
            
            var allBatteries = await _dbContext.Batteries
                .Include(b => b.CurrentHolder)
                .Include(b => b.Mentor)
                .ToListAsync();

            foreach (var battery in allBatteries)
            {
                battery.AssignToHolder(null);
            }

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
