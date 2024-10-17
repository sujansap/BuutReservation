using System.Net.Http.Json;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class TimeSlotService(HttpClient httpClient) : ITimeSlotService
    {
        private readonly HttpClient httpClient = httpClient;

        private const string universalDateFormat = "yyyy-MM-dd";
        public Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDate, DateOnly endDate)
        {
            return httpClient.GetFromJsonAsync<TimeSlotRangeInfoDto>($"TimeSlot/range?startDate={startDate.ToString(universalDateFormat)}&endDate={endDate.ToString(universalDateFormat)}")!;
        }
    }
}