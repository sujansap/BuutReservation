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

            //Right now we don't keep the information of whether a boat is available or not
            var amountOfAvailableBoats = await _dbContext.Boats.CountAsync();

            var date = new DateOnly(year, month, day);

            var timeSlotReservationCounts = await _dbContext.TimeSlots
            .Where(ts => ts.Date == date)
            .Include(ts => ts.Reservations) // Eager load reservations
            .GroupBy(ts => ts) // Group by each TimeSlot
            .Select(g => new
            {
                TimeSlot = g.Key, // The timeslot
                ReservationCount = g.SelectMany(ts => ts.Reservations).Count() // Count of reservations
            })
            .ToListAsync();

            var availableTimeSlots = timeSlotReservationCounts
            .Where(item => item.ReservationCount < amountOfAvailableBoats)
            .Select(item => new TimeSlotDto
            {
                Id = item.TimeSlot.Id,
                Start = item.TimeSlot.Start,
                End = item.TimeSlot.End,
            })
            .ToList();

            //later this needs to change to check weather a timeslot has a reservation that is made by the
            //curent logged in user 
            //what we need to do is check if there is a reservation with userid and timeslotid from availableTimeSlots
            return availableTimeSlots;

        }



    }
}
