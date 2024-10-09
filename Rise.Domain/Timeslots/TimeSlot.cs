using Ardalis.GuardClauses;

using Rise.Domain.Reservations;

namespace Rise.Domain.Timeslots;

public class TimeSlot : Entity, ITimeSlot
{
    private TimeSpan _start;
    private TimeSpan _end;
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

    public TimeSpan Start
    {
        get => _start;
        set
        {
            Guard.Against.OutOfRange(value, nameof(Start), TimeSpan.Zero, TimeSpan.FromHours(24), "Start time must be within a valid range.");

            _start = value;
        }
    }

    public TimeSpan End
    {
        get => _end;
        set
        {
            Guard.Against.OutOfRange(value, nameof(End), TimeSpan.Zero, TimeSpan.FromHours(24), "End time must be within a valid range.");
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