using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System.Data;

namespace Rise.Services.TimeSlots
{
    public class TimeSlotService(ApplicationDbContext dbContext) : ITimeSlotService
    {
        private readonly ApplicationDbContext dbContext = dbContext;

        internal class DateTimeSlotBoatUse
        {
            /// <summary>
            /// The date of the time slot
            /// </summary>
            public DateOnly Date { get; set; }
            /// <summary>
            /// The start time of the time slot
            /// </summary>
            public TimeSpan Start { get; set; }
            /// <summary>
            /// The amount of boats who already have been reserved
            /// </summary>
            public int UsedBoatCount { get; set; }
        }

        public async Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDay, DateOnly endDay)
        {
            Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> daysWithReservation = [];

            List<DateTimeSlotBoatUse> allTimeSlotsDuringRange = await dbContext.TimeSlots.Where(
                timeSlot => startDay <= timeSlot.Date && timeSlot.Date <= endDay
                && !timeSlot.IsDeleted)
                .Select(timeSlot => new DateTimeSlotBoatUse()
                {
                    Date = timeSlot.Date,
                    Start = timeSlot.Start,
                    UsedBoatCount = dbContext.Reservations.Where(reservation => reservation.TimeSlotId == timeSlot.Id).Count()
                })
                .ToListAsync();

                if (allTimeSlotsDuringRange.Count != 0)
                {
                    int totalAmountBoats = dbContext.Boats.Count();

                    daysWithReservation = allTimeSlotsDuringRange
                    .GroupBy(
                        x => x.Date,
                        (date, subset) => new
                        {
                            Date = date,
                            TotalDayTimeSlotCount = subset.DistinctBy(x => x.Start).Count(),
                            UsedBoatCount = subset.Select(x => x.UsedBoatCount).Sum()
                        }
                    ).Select(x =>
                    {
                        bool isFullyBooked = x.TotalDayTimeSlotCount * totalAmountBoats <= x.UsedBoatCount;
                        bool isSlotAvailable = !isFullyBooked && x.Date.CompareTo(DateOnly.FromDateTime(DateTime.Today).AddDays(Reservation.MinDaysBetweenReservation)) > 0;
                        return new TimeSlotDaySurfaceInfoDto(x.Date, isFullyBooked, isSlotAvailable);
                    }).ToDictionary(x => x.Date);
            }

            int totalDays = 1 + endDay.ToDateTime(TimeOnly.MinValue).Subtract(startDay.ToDateTime(TimeOnly.MinValue)).Days;
            IEnumerable<TimeSlotDaySurfaceInfoDto> days = Enumerable.Range(0, totalDays)
                    .Select(offset =>
                        {
                            DateOnly date = startDay.AddDays(offset);
                            return daysWithReservation.GetValueOrDefault(date, new TimeSlotDaySurfaceInfoDto(date, false, false));
                        });

            return new TimeSlotRangeInfoDto(startDay, endDay, totalDays, days);
        }
    }
}
