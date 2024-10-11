namespace Rise.Shared.Timeslots
{
    /// <summary>
    /// The surface info on the reservation state of a day
    /// </summary>
    /// <param name="Date">The date</param>
    /// <param name="IsFullyBooked">If the day is fully booked</param>
    /// <param name="IsSlotAvailable">If the day has a slot available to be booked</param>
    public record class TimeSlotDayInfoDto(DateOnly Date, bool IsFullyBooked, bool IsSlotAvailable)
    {

    }


}