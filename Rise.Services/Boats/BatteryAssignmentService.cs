using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Microsoft.Extensions.Logging;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;

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
            TimeInfo timeInfo = GetCurrentTimeInfo();

            // TODO handle completed reservations
            await HandleCompletedReservations(timeInfo);

            // TODO assign to upcoming reservations
            await AssignBatteriesToReservations(timeInfo);


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
                    .ThenInclude(b => b!.Mentor)
                .Include(r => r.User)
                .Where(r =>
                    r.Battery! != null! &&
                    (r.TimeSlot.Date < timeInfo.Today ||
                    (r.TimeSlot.Date == timeInfo.Today && r.TimeSlot.End <= timeInfo.CurrentTime)))
                .ToListAsync();

            // TODO handle logic for reservation that were canceled
            foreach (Reservation reservation in completedReservations)
            {
                reservation.AssignLastUserToBattery();
            }
        }

        private async Task AssignBatteriesToReservations(TimeInfo timeInfo)
        {
            List<Boat> boats = await _dbContext.Boats
                .Include(boat => boat.Reservations
                    .Where(
                    res =>
                    (timeInfo.Today == res.TimeSlot.Date && timeInfo.CurrentTime < res.TimeSlot.Start) ||
                    (timeInfo.Today < res.TimeSlot.Date && (
                        res.TimeSlot.Date < timeInfo.ThreeDaysFromNow ||
                        (res.TimeSlot.Date == timeInfo.ThreeDaysFromNow && res.TimeSlot.Start < timeInfo.CurrentTime)
                    )))
                    .OrderBy(res => res.TimeSlot.Date)
                    .ThenBy(res => res.TimeSlot.Start)
                )
                    .ThenInclude(res => res.TimeSlot)
                .Include(boat => boat.Reservations.Where(res =>
                    res.Battery! != null!
                ))
                    .ThenInclude(res => res.Battery)
                        .ThenInclude(bat => bat!.Mentor)
                .Include(boat => boat.Reservations)
                    .ThenInclude(res => res.Battery)
                        .ThenInclude(bat => bat!.CurrentHolder)
                .ToListAsync();

            foreach (Boat boat in boats)
            {
                boat.AssignBatteriesToReservations(timeInfo.Now);
            }
        }
    }

    internal record TimeInfo(
    DateTime Now,
    DateOnly Today,
    TimeOnly CurrentTime,
    DateOnly ThreeDaysFromNow);
}
