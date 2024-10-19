using System;

namespace Rise.Shared.Reservations;

public class ReservationListDto
{
    public int Id { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public DateOnly Date { get; set; }
    public int AmountAdults { get; set; }
    public int AmountChildren { get; set; }
    public int AmountPets { get; set; }
}
