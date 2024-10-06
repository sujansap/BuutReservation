namespace Rise.Domain.Timeslots;

public class TimeSlot : Entity
{
    public int CruisePeriodId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public virtual CruisePeriod CruisePeriod { get; set; } = default!;
}
