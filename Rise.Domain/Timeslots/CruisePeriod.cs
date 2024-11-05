

namespace Rise.Domain.Timeslots;
public class CruisePeriod : Entity, ICruisePeriod
{

    private DateTime _start;
    private DateTime _end;

    // TODO make guard clauses for properties of CruisePeriod
    // TODO make tests for constructors CruisePeriod
    // Guard clauses for properties of CruisePeriod
    public DateTime Start
    {
        get => _start;
        set
        {
            Guard.Against.OutOfSQLDateRange(value, nameof(Start));
            _start = value;
        }
    }

    public DateTime End
    {
        get => _end;
        set
        {
            Guard.Against.OutOfRange(value, nameof(End), DateTime.Today, DateTime.MaxValue, "End date must be today or in the future.");
            Guard.Against.OutOfRange(value, nameof(End), _start, DateTime.MaxValue, "End date must be after Start date.");
            _end = value;
        }
    }


    public ICollection<ITimeSlot> TimeSlots { get; set; } = [];
}
