using Rise.Shared.Timeslots;

namespace Rise.Server.Controllers
{
    public interface ITimeSlotService
    {
        /// <summary>
        /// Gets all time slots between given 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="includeCrossOverDays"></param>
        /// <returns></returns>
        Task<TimeSlotRangeInfoDto> GetAllTimeSlots(
            int year,
            int month,
            bool includeCrossOverDays
        );
    }
}