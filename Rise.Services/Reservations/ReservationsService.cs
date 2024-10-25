using System;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Reservations;

namespace Rise.Services.Reservations
{
    public class ReservationsService(ApplicationDbContext dbContext) : IReservationsService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        /// <summary>
        /// Gets all reservations in the given date range by the current user
        /// </summary>
        internal class Reservation
        {
            public DateOnly Date { get; set; }
        }
        public async Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate)
        {
            ISet<DateOnly> reservations = new HashSet<DateOnly>();

            int userId = 2; // This should be the current user id

            List<Reservation> allReservationsDuringRange = await _dbContext.Reservations.Where(
                reservation =>
               reservation.TimeSlot.Date >= startDate &&
                reservation.TimeSlot.Date <= endDate &&
                reservation.UserId == userId
                )
                .Select(reservation => new Reservation()
                {
                    Date = reservation.TimeSlot.Date,
                }
                )
                .ToListAsync();

            return new ReservationsRangeDto(allReservationsDuringRange.Select(reservation => reservation.Date));
        }
    }

}

