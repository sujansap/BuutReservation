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

        public async Task<List<TimeSlotDto>> GetTimeSlotsByDate(DateTime date)
        {
            // Find the CruisePeriod that contains the given date
            var cruisePeriod = await _dbContext.CruisePeriods
                .FirstOrDefaultAsync(cp => cp.Start.Date <= date.Date && cp.End.Date >= date.Date);

            if (cruisePeriod == null)
            {
                return new List<TimeSlotDto>(); // No cruise period found for the given date
            }

            // Fetch the TimeSlots for that CruisePeriod
            var timeSlots = await _dbContext.TimeSlots
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
    }
}
