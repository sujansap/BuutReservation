namespace Rise.Shared.Reservations;
using System.ComponentModel.DataAnnotations;

public record class CreateReservationDto
{

    [Required(ErrorMessage = "TimeSlotId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "TimeSlotId must be greater than zero.")]
    public int TimeSlotId { get; set; }

    [Required(ErrorMessage = "BoatId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "BoatId must be greater than zero.")]
    public int BoatId { get; set; }
}
