using System.Net.Http.Json;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class TimeSlotService(HttpClient httpClient) : ITimeSlotService
    {
        private readonly HttpClient httpClient = httpClient;
        public Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDay, DateOnly endDay)
        {
            return httpClient.GetFromJsonAsync<TimeSlotRangeInfoDto>($"TimeSlot/range?startDay={startDay}&endDay={endDay}")!;
        }
    }
}