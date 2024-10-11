namespace Rise.Shared.TimeSlots
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlotDto>> GetTimeSlotsByDate(DateTime date);
    }
}
