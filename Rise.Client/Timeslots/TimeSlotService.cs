using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Timeslots
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly HttpClient httpClient;

        public TimeSlotService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(DateTime date)
        {
            var timeslots = await httpClient.GetFromJsonAsync<IEnumerable<TimeSlotDto>>($"timeslot?date={date:2024-10-12}");
            return timeslots!;
        }


    }
}