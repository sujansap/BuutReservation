namespace Rise.Shared.Reservations
{
    public interface IReservationsService
    {
        Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId);
    }

}


