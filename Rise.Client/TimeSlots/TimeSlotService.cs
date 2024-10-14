using System.Net.Http.Json;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class TimeSlotService(HttpClient httpClient) : ITimeSlotService
    {
        private readonly HttpClient httpClient = httpClient;
        public Task<TimeSlotRangeInfoDto> GetAllTimeSlotsFromMonth(int year, int month, bool includeCrossOverDays)
        {
            return httpClient.GetFromJsonAsync<TimeSlotRangeInfoDto>($"TimeSlot/{year}/{month}?includeCrossOverDays={includeCrossOverDays}")!;
        }
    }
}