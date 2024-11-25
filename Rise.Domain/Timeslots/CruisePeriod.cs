

namespace Rise.Domain.TimeSlots;
public class CruisePeriod : Entity
{

    private DateTime _start;
    private DateTime _end;

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
            Guard.Against.OutOfSQLDateRange(value, nameof(End));
            Guard.Against.OutOfRange(value, nameof(End), _start, DateTime.MaxValue, "End date must be after Start date.");
            _end = value;
        }
    }
    private readonly List<TimeSlot> timeSlots = [];

    public IReadOnlyList<TimeSlot> TimeSlots => timeSlots.AsReadOnly();

    /// <summary>
    /// Adds a time slot to the given cruise period.
    /// </summary>
    /// <param name="timeSlot">The time slot to add. This should be a valid time within the allowed range of the cruise period.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the time slot's date and start time are out of the valid range specified by the cruise's start and end time.
    /// </exception>
    public void AddTimeSlot(TimeSlot timeSlot)
    {
        Guard.Against.OutOfRange(
            timeSlot.Date.ToDateTime(timeSlot.Start),
            nameof(AddTimeSlot),
            Start,
            End
        );
        timeSlots.Add(timeSlot);
    }
}
