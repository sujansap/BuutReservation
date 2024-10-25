using System;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Reservations;

namespace Rise.Services.Reservations
{
    public class ReservationsService(ApplicationDbContext dbContext) : IReservationsService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        internal class Reservation
        {
            public DateOnly Date { get; set; }
        }
        public async Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId)
        {
            ISet<DateOnly> reservations = new HashSet<DateOnly>();

            List<Reservation> allReservationsDuringRange = await _dbContext.Reservations.Where(
                reservation => startDate <= reservation.TimeSlot.Date && reservation.TimeSlot.Date <= endDate
                && reservation.UserId == userId)
                .Select(reservation => new Reservation()
                {
                    Date = reservation.TimeSlot.Date,
                })
                .ToListAsync();

            return new ReservationsRangeDto(reservations);
        }
    }

}

