namespace Rise.Domain.Timeslots;

public class TimeSlot : Entity
{
    public int CruisePeriodId { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    
    public virtual CruisePeriod CruisePeriod { get; set; } = default!;
}
