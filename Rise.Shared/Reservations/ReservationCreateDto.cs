using System;
using Rise.Shared.TimeSlots;

namespace Rise.Shared.Reservations;

public class ReservationCreateDto
{
  public required TimeSlotDto TimeSlot { get; set; }
}
