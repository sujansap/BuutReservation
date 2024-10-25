using System.Net.Cache;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Reservations;
using Rise.Persistence;
using Rise.Shared.Pagination;
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

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 3)
        {
            IOrderedQueryable<IReservation> reservationsQuery = _dbContext.Reservations
                .Where(r => (r.UserId == userId) && (getPast ?
                   r.TimeSlot.Date < DateOnly.FromDateTime(DateTime.Now) :
                   r.TimeSlot.Date >= DateOnly.FromDateTime(DateTime.Now)))
                .OrderBy(r => r.TimeSlot.Date).ThenBy(r => r.Id);

            int takeAmount = pageSize + 1;

            if (cursor is not null)
            {
                if (isNextPage == true)
                {
                    reservationsQuery = reservationsQuery.Where(r => r.Id > cursor).OrderBy(r => r.TimeSlot.Date).ThenBy(r => r.Id);
                }
                else
                {
                    reservationsQuery = reservationsQuery.Where(r => r.Id < cursor).OrderByDescending(r => r.TimeSlot.Date).ThenBy(r => r.Id);
                    takeAmount = pageSize;
                }
            }

            reservationsQuery = reservationsQuery.Take(takeAmount).OrderBy(r => r.TimeSlot.Date).ThenBy(r => r.Id);

            if (isNextPage == false && cursor is not null)
            {
                reservationsQuery = reservationsQuery.Reverse().OrderByDescending(r => r.TimeSlot.Date).ThenBy(r => r.Id);
            }

            var reservations = await reservationsQuery
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    Start = r.TimeSlot.Start,
                    End = r.TimeSlot.End,
                    Date = r.TimeSlot.Date,
                    BoatId = r.BoatId,
                    BoatPersonalName = r.Boat.PersonalName
                })
                .ToListAsync();

            if (reservations.Count == 0)
            {
                return new ItemsPageDto<ReservationDto>
                {
                    Data = [],
                    IsFirstPage = true,
                    NextId = null,
                    PreviousId = null
                };
            }

            bool isFirstPage = !cursor.HasValue ||
                                (cursor.HasValue && reservations.OrderBy(r => r.Date).ThenBy(r => r.Id).FirstOrDefault()?.Id == _dbContext.Reservations.OrderBy(r => r.TimeSlot.Date).ThenBy(r => r.Id).FirstOrDefault()?.Id);

            bool hasNextPage = reservations.Count > pageSize ||
                                (cursor is not null && isNextPage == false);

            if (reservations.Count > pageSize)
            {
                reservations.RemoveAt(reservations.Count - 1);
            }

            int? nextId = hasNextPage
                    ? reservations.OrderBy(r => r.Date).ThenBy(r => r.Id).LastOrDefault()?.Id
                    : null;

            int? previousId = reservations.Count > 0 && !isFirstPage
                    ? reservations.OrderBy(r => r.Date).ThenBy(r => r.Id).FirstOrDefault()?.Id
                    : null;

            return new ItemsPageDto<ReservationDto>
            {
                Data = reservations,
                IsFirstPage = isFirstPage,
                NextId = nextId,
                PreviousId = previousId
            };
        }
    }

}