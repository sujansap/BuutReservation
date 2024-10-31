using Rise.Shared.Pagination;

namespace Rise.Shared.Reservations
{
    public interface IReservationService
    {
        Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId);

        /// <summary>
        /// Gets all the upcoming reservations for the current user
        /// </summary>
        /// <returns>All the reservations</returns>

        Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5);


        /// <summary>
        /// Creates a reservation for the given timeslot ID
        /// </summary>
        /// <param name="timeslotId">The ID of the timeslot to reserve</param>
        /// <returns>The created reservation details</returns>
        Task<ReservationDto> CreateReservation(int timeSlotId);
    }
}