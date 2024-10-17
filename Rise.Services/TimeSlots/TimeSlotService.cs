using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.TimeSlots;

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

            //we get all the timeslots for a given date and the amount of reservations for that timeslot
            var timeSlotReservationCounts = await _dbContext.TimeSlots
            .Where(ts => ts.Date == date)
            .Select(ts => new
            {
                TimeSlot = ts, // The timeslot itself
                ReservationCount = ts.Reservations.Count() // how many times this timeslot is reserved
            })
            .ToListAsync();

            //for debugging purposes
            timeSlotReservationCounts.ForEach(item => Console.WriteLine(item.ReservationCount));

            //if the timeslot is available we add it to the availableTimeSlots list
            //we check whether the amount of reservations is less than the amount of boats for a given timeslot
            var availableTimeSlots = timeSlotReservationCounts
            .Where(item => item.ReservationCount < amountOfAvailableBoats)
            .Select(item => new TimeSlotDto
            {
                Id = item.TimeSlot.Id,
                Start = item.TimeSlot.Start,
                End = item.TimeSlot.End,
            })
            .OrderBy(item => item.Start)
            .ToList();

            //later this needs to change to check whether a timeslot has a reservation that is made by the
            //curent logged in user 
            //what we need to do is check if there is a reservation with userid and timeslotid from availableTimeSlots
            return availableTimeSlots;
        }


    }
}
