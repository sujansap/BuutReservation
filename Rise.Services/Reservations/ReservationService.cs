using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;
using Rise.Persistence;
using Rise.Shared.Reservations;

namespace Rise.Services.Reservations
{

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<ReservationListDto>> GetCurrentUserReservations(int userId)
        {

            return await _dbContext.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Reservations)
                .Select(r => new ReservationListDto
                {
                    Id = ((Reservation)r).Id,
                    AmountAdults = r.AmountAdults,
                    AmountChildren = r.AmountChildren,
                    AmountPets = r.AmountPets,
                    Start = r.TimeSlot.Start,
                    End = r.TimeSlot.End,
                    Date = r.TimeSlot.Date
                })
                .ToListAsync();
        }

    }

}