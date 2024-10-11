using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System.Data;

namespace Rise.Services.TimeSlots
{
    public class TimeSlotService(ApplicationDbContext dbContext) : ITimeSlotService
    {
        private readonly ApplicationDbContext dbContext = dbContext;

        public async Task<List<TimeSlotDto>> GetTimeSlotsByDate(DateTime date)
        {
            // Find the CruisePeriod that contains the given date
            var cruisePeriod = await dbContext.CruisePeriods
                .FirstOrDefaultAsync(cp => cp.Start.Date <= date.Date && cp.End.Date >= date.Date);

            if (cruisePeriod == null)
            {
                return new List<TimeSlotDto>(); // No cruise period found for the given date
            }

            // Fetch the TimeSlots for that CruisePeriod
            var timeSlots = await dbContext.TimeSlots
                .Where(ts => ts.CruisePeriodId == cruisePeriod.Id)
                .Select(ts => new TimeSlotDto
                {
                    Id = ts.Id,
                    Start = ts.Start,
                    End = ts.End,
                    CruisePeriodId = ts.CruisePeriodId
                })
                .ToListAsync();

            return timeSlots;
        }

        public async Task<TimeSlotRangeInfoDto> GetAllTimeSlotsFromMonth(
            int year,
            int month,
            bool includeCrossOverDays)
        {
            // TODO double check persistence if queries can be indexed better
            // TODO query can be reused GetTimeSlotsByDate
            (DateOnly startDay, DateOnly endDay) = GenerateDayRange(year, month, includeCrossOverDays);
            Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> daysWithReservation = [];

            var overlappingCruisePeriod = await dbContext.CruisePeriods
                .FirstOrDefaultAsync(cruisePeriod => (DateOnly.FromDateTime(cruisePeriod.Start.Date) <= startDay && startDay <= DateOnly.FromDateTime(cruisePeriod.End.Date)) ||
                (DateOnly.FromDateTime(cruisePeriod.Start.Date) <= endDay && endDay <= DateOnly.FromDateTime(cruisePeriod.End.Date)));

            var allTimeSlotsDuringRange = await dbContext.TimeSlots.Where(
                timeSlot => startDay <= timeSlot.Date && timeSlot.Date <= endDay
                && !timeSlot.IsDeleted)
                .Select(timeSlot => new
                {
                    timeSlot.Date,
                    timeSlot.Start,
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
                    bool IsFullyBooked = x.TotalDayTimeSlotCount * totalAmountBoats <= x.UsedBoatCount;
                    return new TimeSlotDaySurfaceInfoDto(x.Date, IsFullyBooked, !IsFullyBooked);
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

        // TODO documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="includeCrossOverDays"></param>
        /// <returns></returns>
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

        private static int NormalDayIndexToMonday(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }
    }
}
