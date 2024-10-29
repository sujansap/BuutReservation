using System;
using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components;

public partial class CustomCalendar
{
  [Parameter]
  public Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> AvailableDays { get; set; } = [];
}
