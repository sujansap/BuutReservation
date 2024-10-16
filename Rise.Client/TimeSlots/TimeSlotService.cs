using System.Net.Http.Json;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class TimeSlotService(HttpClient httpClient) : ITimeSlotService
    {
        private readonly HttpClient httpClient = httpClient;
        // public Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDay, DateOnly endDay)
        // {
        //     return httpClient.GetFromJsonAsync<TimeSlotRangeInfoDto>($"TimeSlot/range?startDay={startDay}&endDay={endDay}")!;
        // }

        public Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDay, DateOnly endDay)
        {
            var random = new Random();

            // Generate mock data for each day in the range
            var mockDays = new List<TimeSlotDaySurfaceInfoDto>();
            for (var date = startDay; date <= endDay; date = date.AddDays(1))
            {
                bool isFullyBooked = random.Next(2) == 0;
                bool isSlotAvailable = random.Next(2) == 0;

                mockDays.Add(new TimeSlotDaySurfaceInfoDto(
                    Date: date,
                    IsFullyBooked: isFullyBooked,
                    IsSlotAvailable: isSlotAvailable
                ));
            }

            // Calculate total days
            int totalDays = (endDay.DayNumber - startDay.DayNumber) + 1;

            // Create the mock TimeSlotRangeInfoDto
            var mockResponse = new TimeSlotRangeInfoDto(
                Start: startDay,
                End: endDay,
                TotalDays: totalDays,
                Days: mockDays
            );

            // Return the mock data as a completed task
            return Task.FromResult(mockResponse);
        }
    }
}