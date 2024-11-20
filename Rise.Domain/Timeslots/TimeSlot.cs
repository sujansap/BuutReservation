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
            Guard.Against.OutOfRange(value.Year, nameof(Date), 2000, 9999, 
                "TimeSlot date must be after year 2000.");

            if (CruisePeriod != null)
            {
                DateOnly startDate = DateOnly.FromDateTime(CruisePeriod.Start);
                DateOnly endDate = DateOnly.FromDateTime(CruisePeriod.End);
                
                Guard.Against.OutOfRange(value, nameof(Date), startDate, endDate,
                    "TimeSlot date must be within the CruisePeriod's date range.");
            }
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
            Guard.Against.OutOfRange(value, nameof(End), TimeOnly.MinValue, TimeOnly.MaxValue, 
                "End time must be within a valid range.");
            Guard.Against.InvalidInput(value, nameof(End), 
                x => _start == default || x > _start,
                "End time must be after Start time.");
            _end = value;
        }
    }

    public int CruisePeriodId { get; set; }
    public ICruisePeriod CruisePeriod { get; set; } = default!;

    public ICollection<IReservation> Reservations { get; } = [];
}