namespace Rise.Shared.TimeSlots
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(DateTime date);
    }
}
