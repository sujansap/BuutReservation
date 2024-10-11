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
        /// <param name="includeCrossOverDays"></param>
        /// <returns></returns>
        Task<TimeSlotRangeInfoDto> GetAllTimeSlotsFromMonth(
            int year,
            int month,
            bool includeCrossOverDays
        );
    }
}
