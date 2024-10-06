

namespace Rise.Domain.Timeslots;
public class CruisePeriod : Entity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
