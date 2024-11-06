using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.Dialogs.DialogContent
{
    public partial class CreatereservationSuccessConent
    {
        [Parameter] public required UserDto User { get; set; }
        [Parameter] public required DateOnly Date { get; set; }
        [Parameter] public required TimeSlotDto? TimeSlot { get; set; }
    }
}