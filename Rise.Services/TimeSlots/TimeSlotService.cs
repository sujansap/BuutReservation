using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Services.TimeSlots
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ApplicationDbContext _dbContext;

        public TimeSlotService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(int year, int month, int day)
        {
            var date = new DateOnly(year, month, day);

            var timeSlots = await _dbContext.TimeSlots.Where(ts => ts.Date == date).
            Select(ts => new TimeSlotDto
            {
                Id = ts.Id,
                Start = ts.Start,
                End = ts.End
            }).ToListAsync();

            return timeSlots;

        }
    }
}
