namespace Rise.Shared.Reservations;

public record class CreateReservationDto
{
    public int UserId { get; set; }
    public int TimeSlotId { get; set; }
    public int BoatId { get; set; }
}
