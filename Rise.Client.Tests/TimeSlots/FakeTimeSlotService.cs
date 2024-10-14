using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class FakeTimeSlotService : ITimeSlotService
    {
        public Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(int year, int month, int day)
        {
            var timeSlots = Enumerable.Range(1, 5)
                                      .Select(i => new TimeSlotDto
                                      {
                                          Id = i,
                                          Start = new TimeSpan(8 + i, 0, 0),
                                          End = new TimeSpan(9 + i, 0, 0),
                                          CruisePeriodId = i
                                      });

            return Task.FromResult(timeSlots);
        }
    }
}