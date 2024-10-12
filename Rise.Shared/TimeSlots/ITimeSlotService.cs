namespace Rise.Shared.TimeSlots
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlotDto>> GetTimeSlotsByDate(DateTime date);

        /// <summary>
        /// Gets all time slots between given 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="includeCrossOverDays">If days need to be included from the weeks where in the month crosses over from/into the other</param>
        /// <returns>All time slots with there general info</returns>
        Task<TimeSlotRangeInfoDto> GetAllTimeSlotsFromMonth(
            int year,
            int month,
            bool includeCrossOverDays
        );
    }
}
