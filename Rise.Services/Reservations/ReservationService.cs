using System.Linq.Expressions;
using System.Net.Cache;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Common;
using Rise.Domain.Exceptions;
using Rise.Domain.Reservations;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Pagination;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Services.Reservations
{
    public class ReservationService(ApplicationDbContext dbContext) : IReservationService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        /// <summary>
        /// Gets all reservations in the given date range by the current user
        /// </summary>
        internal class ReservationTemp
        {
            public DateOnly Date { get; set; }
        }
        public async Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId)
        {
            ISet<DateOnly> reservations = new HashSet<DateOnly>();

            List<ReservationTemp> allReservationsDuringRange = await _dbContext.Reservations.Where(
                reservation =>
                reservation.TimeSlot.Date >= startDate &&
                reservation.TimeSlot.Date <= endDate &&
                reservation.UserId == userId
                )
                .Select(reservation => new ReservationTemp()
                {
                    Date = reservation.TimeSlot.Date,
                }
                )
                .ToListAsync();

            return new ReservationsRangeDto(allReservationsDuringRange.Select(reservation => reservation.Date));
        }

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5)
        {
            return await PaginationService.GetPaginatedResultsAsync<IReservation, ReservationDto>(
                queryableDbSet: _dbContext.Reservations.AsQueryable(),
                filterLambda: r => (r.UserId == userId) && (getPast ?
                   r.TimeSlot.Date < DateOnly.FromDateTime(DateTime.Now) :
                   r.TimeSlot.Date >= DateOnly.FromDateTime(DateTime.Now)),
                orderingExpressions: [
                new OrderingExpression<IReservation, object> {
                    OrderLambda = r => r.TimeSlot.Date
                },
                new OrderingExpression<IReservation, object> {
                    OrderLambda = r => r.Id
                 }
                ],
                projection: r => new ReservationDto
                {
                    Id = r.Id,
                    Start = r.TimeSlot.Start,
                    End = r.TimeSlot.End,
                    Date = r.TimeSlot.Date,
                    BoatId = r.BoatId,
                    BoatPersonalName = r.Boat.PersonalName
                },
                cursor: cursor,
                isNextPage: isNextPage,
                pageSize: pageSize
            );
        }

        /// <summary>
        /// Creates a reservation for the current user
        /// </summary>
        /// <param name="timeSlotId"></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="NoEntityAvailableException"></exception>
        /// <exception cref="EntityAlreadyExistsException"></exception>
        /// <exception cref="ReservationCreationFailedException"></exception>
        public async Task<int> CreateReservation(CreateReservationDto reservationDto)
        {
            var userId = 2; //get this from session or token later

            var user = await _dbContext.Users.FindAsync(userId);

            if (user is null)
            {
                throw new EntityNotFoundException(nameof(User), userId);
            }

            var timeSlot = await _dbContext.TimeSlots.FindAsync(reservationDto.TimeSlotId);

            if (timeSlot is null)
            {
                throw new EntityNotFoundException(nameof(TimeSlot), reservationDto.TimeSlotId);
            }

            //get a boat that is available for that timeslot
            //we just assign the first boat that is available
            //user can't choose a boat
            var boat = await _dbContext.Boats.Where(b => b.Reservations.All(r => r.TimeSlotId != timeSlot.Id)).FirstOrDefaultAsync();

            if (boat is null)
            {
                throw new NoBoatAvailableException(timeSlot.Id);
            }
            var boatId = boat.Id;

            var reservation = new Reservation
            {
                UserId = userId,
                User = user,
                TimeSlot = timeSlot,
                TimeSlotId = timeSlot.Id,
                BoatId = boatId,
                Boat = boat
            };

            try
            {
                _dbContext.Reservations.Add(reservation);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ReservationCreationFailedException("Failed to create reservation.");
            }

            return reservation.Id;
        }
    }
}
