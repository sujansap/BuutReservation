
namespace Rise.Shared.Reservations
{
    public interface IReservationService
    {
        /// <summary>
        /// Gets all the upcoming reservations for the current user
        /// </summary>
        /// <returns>All the reservations</returns>
        Task<IEnumerable<ReservationListDto>> GetCurrentUserReservations(int userId);

    }
}