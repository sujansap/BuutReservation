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
        /// Creates a reservation with given details
        /// </summary>
        /// <param name="reservationDto">Dto with details of to be created reservation</param>
        /// <returns>The created reservation details</returns>
        Task<int> CreateReservation(CreateReservationDto reservationDto);
    }
}