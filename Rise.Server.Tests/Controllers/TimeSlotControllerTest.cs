using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.TimeSlots;
using System.Net.Http.Json;

namespace Rise.Server.Tests.Controllers
{
    public class TimeSlotControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture)
    {
        [Fact]
        public async Task GET_TimeSlotsByDate_GivesTimeSlots()
        {

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day + 1;


            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"TimeSlot/{year}/{month}/{day}"))!;

            // Assert
            response.ShouldNotBeEmpty();
            response.Count.ShouldBe(2);
            Console.WriteLine(response);
            response.ShouldContain(ts => ts.Start.Equals(TimeSpan.Parse("14:00:00")) && ts.End.Equals(TimeSpan.Parse("17:00:00")));

            response.ShouldContain(ts => ts.Start.Equals(TimeSpan.Parse("18:00:00")) && ts.End.Equals(TimeSpan.Parse("21:00:00")));

        }



        [Theory]
        [InlineData(8)]
        [InlineData(9)]
        public async Task GET_TimeSlotsByDate_GivesNoTimeSlots(int daysFromNow)
        {

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = (DateTime.Now.Day + daysFromNow) % 30; // 8 or 9 days from now, no timeslots should be available


            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"TimeSlot/{year}/{month}/{day}"))!;

            // Assert
            response.ShouldBeEmpty();
            response.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GET_TimeSlotsByDate_InvalidDate_ReturnsBadRequest()
        {

            int year = DateTime.Now.Year;
            int month = 13; // month is invalid, doesn't exist
            int day = DateTime.Now.Day + 1;


            var response = await _client.GetAsync($"TimeSlot/{year}/{month}/{day}");
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}