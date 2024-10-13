using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly HttpClient httpClient;

        public TimeSlotService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(int year, int month, int day)
        {
            var timeslots = await httpClient.GetFromJsonAsync<IEnumerable<TimeSlotDto>>($"TimeSlot/{year}/{month}/{day}");
            return timeslots!;
        }


    }
}