using System;

namespace Rise.Shared.TimeSlots
{
    public interface ICruisePeriodService
    {
        /// <summary>
        /// Gets cruise period by id
        /// </summary>
        /// <param name="id">cruise period id</param>
        /// <returns>Cruise period in full detail</returns>
        Task<CruisePeriodDetailedDto> GetCruisePeriod(int id);
    }
}
