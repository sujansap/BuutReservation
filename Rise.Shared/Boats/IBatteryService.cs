namespace Rise.Shared.Boats
{
    public interface IBatteryService
    {
        /// <summary>
        /// Update the battery with new info
        /// </summary>
        /// <param name="id">the id of the battery</param>
        /// <param name="newBattery">new battery info</param>
        /// <returns>The new version of the battery</returns>
        /// <exception cref="EntityNotFoundException">When the battery or mentor could not be found</exception>
        Task<BatteryDto> UpdateBattery(int id, BatteryUpdateDto newBattery);
    }

}