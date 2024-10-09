namespace Rise.Domain.Timeslots;

public class TimeSlot : Entity, ITimeSlot
{
    // TODO make guard clauses for properties of TimeSlot
    // TODO make tests for constructors TimeSlot
    public DateOnly Date { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public int CruisePeriodId { get; set; }
    public ICruisePeriod CruisePeriod { get; set; } = default!;
}
