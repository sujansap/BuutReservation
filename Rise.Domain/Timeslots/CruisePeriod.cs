

namespace Rise.Domain.Timeslots;
public class CruisePeriod : Entity, ICruisePeriod
{

    // TODO make guard clauses for properties of CruisePeriod
    // TODO make tests for constructors CruisePeriod
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public ICollection<ITimeSlot> TimeSlots { get; set; } = [];
}
