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

        // TODO move to CruisePeriod Service?
        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <returns>If there is a cruise period active in given date range</returns>
        private Task<CruisePeriod?> CruisePeriodInDateRange(DateTime start, DateTime end)
        {
            return dbContext.CruisePeriods
                .FirstOrDefaultAsync(cruisePeriod => cruisePeriod.Start.Date <= end && start <= cruisePeriod.End.Date);
        }

        public async Task<TimeSlotRangeInfoDto> GetAllTimeSlotsFromMonth(
            int year,
            int month,
            bool includeCrossOverDays)
        {
            (DateOnly startDay, DateOnly endDay) = GenerateDayRange(year, month, includeCrossOverDays);
            Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> daysWithReservation = [];

            CruisePeriod? overlappingCruisePeriod = await CruisePeriodInDateRange(
                startDay.ToDateTime(TimeOnly.MinValue),
                endDay.ToDateTime(TimeOnly.MaxValue)
            );

            if (overlappingCruisePeriod is not null)
            {
                List<DateTimeSlotBoatUse> allTimeSlotsDuringRange = await dbContext.TimeSlots.Where(
                timeSlot => timeSlot.CruisePeriodId == overlappingCruisePeriod.Id
                && startDay <= timeSlot.Date && timeSlot.Date <= endDay
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

        /// <summary>
        ///  Gets the start and end day of a month with the possibility
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="includeCrossOverDays">If days need to be included from the weeks where in the month crosses over from/into the other</param>
        /// <returns>Start day and end day</returns>
        private static (DateOnly, DateOnly) GenerateDayRange(int year, int month, bool includeCrossOverDays)
        {
            // ? use local for calendars where Sunday is start of the week
            DateOnly firstDayMonth = new(year, month, 1);
            DateOnly lastDayMonth = firstDayMonth.AddMonths(1).AddDays(-1);

            int firstDayIndex = NormalDayIndexToMonday(firstDayMonth.DayOfWeek);
            int lastDayIndex = NormalDayIndexToMonday(lastDayMonth.DayOfWeek);

            DateOnly startDay = includeCrossOverDays ? firstDayMonth.AddDays(1 - firstDayIndex) : firstDayMonth;
            DateOnly endDay = includeCrossOverDays ? lastDayMonth.AddDays(7 - lastDayIndex) : lastDayMonth;

            return (startDay, endDay);
        }

        /// <summary>
        /// Shifts start day of the week to Monday in place of the default Sunday
        /// </summary>
        /// <param name="dayOfWeek">Which day of the week to shift</param>
        /// <returns>Index of the day of the week starting from Monday (index 1)</returns>
        private static int NormalDayIndexToMonday(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }
    }
}
