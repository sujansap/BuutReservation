using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Task<TimeSlotRangeInfoDto> GetAllTimeSlots(
            int year,
            int month,
            bool includeCrossOverDays)
        {
            var (startDay, endDay) = GenerateDayRange(year, month, includeCrossOverDays);
            var task = new Task<TimeSlotRangeInfoDto>(
                () =>
                {
                    int totalDays = 1 + endDay.Subtract(startDay).Days;
                    IEnumerable<TimeSlotDayInfoDto> days = Enumerable.Range(0, totalDays)
                    .Select(offset =>
                    {
                        var date = DateOnly.FromDateTime(startDay.AddDays(offset).Date);
                        return new TimeSlotDayInfoDto(date, true, true);
                    });
                    return new TimeSlotRangeInfoDto(DateOnly.FromDateTime(startDay), DateOnly.FromDateTime(endDay), totalDays, days);
                }
            );
            task.Start();
            return task;
        }

        // TODO documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="includeCrossOverDays"></param>
        /// <returns></returns>
        private static (DateTime, DateTime) GenerateDayRange(int year, int month, bool includeCrossOverDays)
        {
            // ? use local for calendars where Sunday is start of the week
            DateOnly firstDayMonth = new(year, month, 1);
            DateOnly lastDayMonth = firstDayMonth.AddMonths(1).AddDays(-1);

            int firstDayIndex = NormalDayIndexToMonday(firstDayMonth.DayOfWeek);
            int lastDayIndex = NormalDayIndexToMonday(lastDayMonth.DayOfWeek);

            DateOnly startDay = includeCrossOverDays ? firstDayMonth.AddDays(1 - firstDayIndex) : firstDayMonth;
            DateOnly endDay = includeCrossOverDays ? lastDayMonth.AddDays(7 - lastDayIndex) : lastDayMonth;

            return (startDay.ToDateTime(new TimeOnly(0)), endDay.ToDateTime(new TimeOnly(0)));
        }

        private static int NormalDayIndexToMonday(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }
    }
}
