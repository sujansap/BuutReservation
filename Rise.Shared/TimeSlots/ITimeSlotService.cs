namespace Rise.Shared.TimeSlots
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(
            int year,
            int month,
            int day);
    }
}
