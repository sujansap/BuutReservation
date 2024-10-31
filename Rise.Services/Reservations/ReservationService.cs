using System.Linq.Expressions;
using System.Net.Cache;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
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


        public async Task<ReservationDto> CreateReservation(int timeSlotId)
        {
            var userId = 2; //get this from session or token later

            //get a boat that is available for that timeslot
            //we just assign the first boat that is available
            //user can't choose a boat
            var boat = await _dbContext.Boats.Where(b => b.Reservations.All(r => r.TimeSlotId != timeSlotId)).FirstOrDefaultAsync();

            if (boat is null)
            {
                throw new ArgumentException("No boat available for that time slot");
            }
            var boatId = boat.Id;




            if (boat is null)
            {
                throw new ArgumentException("Boat not found");
            }

            var timeSlot = await _dbContext.TimeSlots.FindAsync(timeSlotId);
            if (timeSlot is null)
            {
                throw new ArgumentException("Time slot not found");
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user is null)
            {
                throw new ArgumentException("User not found");
            }

            var reservation = new Reservation
            {
                UserId = userId,
                User = user,
                TimeSlot = timeSlot,
                TimeSlotId = timeSlotId,
                BoatId = boatId,
                Boat = boat
            };


            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();


            return new ReservationDto
            {
                Id = reservation.Id,
                Start = timeSlot.Start,
                End = timeSlot.End,
                Date = timeSlot.Date,
                BoatId = boatId,
                BoatPersonalName = boat.PersonalName
            };


        }
    }
}
