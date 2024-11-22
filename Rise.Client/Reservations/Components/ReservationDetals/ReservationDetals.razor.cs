using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client.Common;
using Rise.Client.Localization.Reservations;
using Rise.Shared;
using Rise.Shared.Reservations;

namespace Rise.Client.Reservations.Components.ReservationDetals
{
    public partial class ReservationDetals : ComponentBase
    {

        public required AsyncData<ReservationDetailsDto> AsyncDataRef { get; set; }
        protected ReservationDetailsDto? ReservationDetails { get; set; }


        [Parameter]
        public int Id { get; set; }

        [Inject]
        public required IReservationService ReservationService { get; set; }

        protected Task<ReservationDetailsDto> GetReservationDetails()
        {
            return ReservationService.GetReservationDetailsAsync(Id);
        }

    }
}