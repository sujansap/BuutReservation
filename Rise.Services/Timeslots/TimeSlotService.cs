using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rise.Persistence;
using Rise.Server.Controllers;
using Rise.Shared.Timeslots;

namespace Rise.Services.Timeslots
{
    public class TimeSlotService(ApplicationDbContext dbContext) : ITimeSlotService
    {
        private readonly ApplicationDbContext dbContext = dbContext;
        public Task<TimeSlotRangeInfoDto> GetAllTimeSlots(int year,
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