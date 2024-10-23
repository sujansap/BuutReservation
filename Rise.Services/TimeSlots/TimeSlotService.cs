using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Reservations;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System.Data;

namespace Rise.Services.TimeSlots
{
    public class TimeSlotService(ApplicationDbContext dbContext) : ITimeSlotService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

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

        public async Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDate, DateOnly endDate)
        {
            Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> daysWithReservation = [];

            List<DateTimeSlotBoatUse> allTimeSlotsDuringRange = await _dbContext.TimeSlots.Where(
                timeSlot => startDate <= timeSlot.Date && timeSlot.Date <= endDate
                && !timeSlot.IsDeleted)
                .Select(timeSlot => new DateTimeSlotBoatUse()
                {
                    Date = timeSlot.Date,
                    Start = timeSlot.Start,
                    UsedBoatCount = _dbContext.Reservations.Where(reservation => reservation.TimeSlotId == timeSlot.Id).Count()
                })
                .ToListAsync();

            if (allTimeSlotsDuringRange.Count != 0)
            {
                int totalAmountBoats = _dbContext.Boats.Count();

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

            int totalDays = 1 + endDate.ToDateTime(TimeOnly.MinValue).Subtract(startDate.ToDateTime(TimeOnly.MinValue)).Days;
            IEnumerable<TimeSlotDaySurfaceInfoDto> days = Enumerable.Range(0, totalDays)
                    .Select(offset =>
                        {
                            DateOnly date = startDate.AddDays(offset);
                            return daysWithReservation.GetValueOrDefault(date, new TimeSlotDaySurfaceInfoDto(date, false, false));
                        });

            return new TimeSlotRangeInfoDto(startDate, endDate, totalDays, days);

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
