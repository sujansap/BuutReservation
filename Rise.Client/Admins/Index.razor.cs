using Microsoft.AspNetCore.Components;
using Rise.Shared.Reservations;


namespace Rise.Client.Admins
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private IReservationService ReservationService { get; set; } = default!;

        private int _todayReservationsCount;
        private bool _loading = true;
        private string? _error;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                _todayReservationsCount = await ReservationService.GetReservationsCountAsync(today);
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



