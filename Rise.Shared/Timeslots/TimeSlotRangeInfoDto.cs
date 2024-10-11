namespace Rise.Shared.Timeslots
{
    /// <summary>
    /// The surface info on the reservation states of day range
    /// </summary>
    /// <param name="Start">From what day the range starts</param>
    /// <param name="End">From what day the range ends</param>
    /// <param name="TotalDays">Amount of days in the range</param>
    /// <param name="Days">Reservation states of every day in the range</param>
    public record class TimeSlotRangeInfoDto(DateOnly Start, DateOnly End, int TotalDays, IEnumerable<TimeSlotDayInfoDto> Days)
    {

    }
}