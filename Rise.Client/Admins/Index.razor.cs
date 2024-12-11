using Microsoft.AspNetCore.Components;
using Rise.Shared.Reservations;
using Rise.Shared.Users;


namespace Rise.Client.Admins
{
    public partial class Index
    {
        [Inject]
        private IReservationService ReservationService { get; set; } = default!;

        [Inject]
        private IUserAdminService UserService { get; set; } = default!;

        private int _todayReservationsCount;
        private int _activeUsersCount;
        private bool _loading = true;
        private string? _error;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var reservationsTask = ReservationService.GetReservationsCountAsync(today);
                var usersTask = UserService.GetActiveUsersCountAsync();

                await Task.WhenAll(reservationsTask, usersTask);

                _todayReservationsCount = await reservationsTask;
                _activeUsersCount = await usersTask;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            finally
            {
                _loading = false;
            }
        }
    }


}



