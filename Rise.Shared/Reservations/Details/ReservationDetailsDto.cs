namespace Rise.Shared.Reservations;

public class ReservationDetailsDto : BaseDto
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public DateOnly Date { get; set; }
    public int BoatId { get; set; }
    public string BoatPersonalName { get; set; } = default!;

    public string? MentorName { get; set; }

    public bool IsDeleted { get; set; }

    public string? BatteryType { get; set; }
    public int? BatteryId { get; set; }
    public string? CurrentBatteryUserName { get; set; }
    public int? CurrentBatteryUserId { get; set; }
}
