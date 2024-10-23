using Ardalis.GuardClauses;
using Rise.Domain.Reservations;

// using Rise.Domain.Reservations;

namespace Rise.Domain.Timeslots;

public class TimeSlot : Entity, ITimeSlot
{
    private TimeOnly _start;
    private TimeOnly _end;
    private DateOnly _date;

    public DateOnly Date
    {
        get => _date;
        set
        {
            Guard.Against.OutOfRange(value, nameof(Date), DateOnly.FromDateTime(DateTime.Today), DateOnly.MaxValue, "Date must be today or in the future.");
            _date = value;
        }
    }

    public TimeOnly Start
    {
        get => _start;
        set
        {
            Guard.Against.OutOfRange(value, nameof(Start), TimeOnly.MinValue, TimeOnly.MaxValue, "Start time must be within a valid range.");

            _start = value;
        }
    }

    public TimeOnly End
    {
        get => _end;
        set
        {
            Guard.Against.OutOfRange(value, nameof(End), TimeOnly.MinValue, TimeOnly.MaxValue, "End time must be within a valid range.");
            if (_start != default && value <= _start)
            {
                throw new ArgumentOutOfRangeException(nameof(End), "End time must be after Start time.");
            }
            _end = value;
        }
    }

    public int CruisePeriodId { get; set; }
    public ICruisePeriod CruisePeriod { get; set; } = default!;

    public ICollection<IReservation> Reservations { get; } = [];
}