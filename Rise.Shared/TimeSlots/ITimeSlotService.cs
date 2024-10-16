namespace Rise.Shared.TimeSlots
{
    public interface ITimeSlotService
    {

        /// <summary>
        /// Gets all time slots between given range
        /// </summary>
        /// <param name="startDay">Date from where the range starts</param>
        /// <param name="endDay">Date from where the range ends (inclusive)</param>
        /// <returns>All time slots with their general info</returns>
        Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDay, DateOnly endDay);
    }
}
