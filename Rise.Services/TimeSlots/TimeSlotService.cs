using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System.Data;
using Rise.Domain.Users;

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
            int userId = 2; // This should be the current user id

            var date = new DateOnly(year, month, day);
            var today = DateOnly.FromDateTime(DateTime.Today);
            var minReservationDate = GetMinReservationDate();

            // Right now we don't keep the information of whether a boat is available or not
            var amountOfAvailableBoats = await _dbContext.Boats.CountAsync();


            var availableTimeSlots = await _dbContext.TimeSlots
                .Where(ts => ts.Date == date) // Filter by the given date, we only want the time slots for that day
                .Select(ts => new
                {
                    TimeSlot = ts,
                    ReservationCount = ts.Reservations.Count(),
                    IsBookedByUser = ts.Reservations.Any(r => r.Users.Any(u => ((User)u).Id == userId))
                })
                .Where(item =>
                    (date >= today && date <= minReservationDate && item.IsBookedByUser) || // Case 1: Date between today and minReservationDate, only booked by user (because you can't book between today and minReservationDate)
                    (date > minReservationDate && (item.ReservationCount < amountOfAvailableBoats || item.IsBookedByUser)) // Case 2: Date >= minReservationDate, available or booked by user (because you can book from after minReservationDate onwards)
                )
                .Select(item => new TimeSlotDto
                {
                    Id = item.TimeSlot.Id,
                    Start = item.TimeSlot.Start,
                    End = item.TimeSlot.End,
                    IsBookedByUser = item.IsBookedByUser
                })
                .OrderBy(item => item.Start) // Order by start time
                .ToListAsync();


            return availableTimeSlots;
        }



        /// <summary>
        /// Calculates the date from which a reservation can be made
        /// </summary>
        public static DateOnly GetMinReservationDate()
        {
            // Calculate the date based on today's date plus the minimum days
            return DateOnly.FromDateTime(DateTime.Today.AddDays(Reservation.MinDaysBetweenReservation));

        }


    }
}
