using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.TimeSlots;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Rise.Server.Tests.Controllers
{
    public class TimeSlotControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture)
    {
        private const string universalDateFormat = "yyyy-MM-dd";
        private const string defaultValidationErrorTitle = "One or more validation errors occurred.";

        private static string DateOnlyToUniversalDate(DateOnly date)
        {
            return date.ToString(universalDateFormat);
        }
        private static string MakeTimeSlotRangeUrl(DateOnly? startDate, DateOnly? endDate)
        {
            List<string> queries = [];
            if (startDate is not null) queries.Add($"startDate={DateOnlyToUniversalDate((DateOnly)startDate)}");
            if (endDate is not null) queries.Add($"endDate={DateOnlyToUniversalDate((DateOnly)endDate)}");

            string joinedQueries = queries.Count == 0 ? "" : ("?" + string.Join("&", queries));

            return $"TimeSlot/range{joinedQueries}";
        }

        [Fact]
        public async Task GET_ValidDateRange_GivesDates()
        {
            int daysDifference = 7;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endDate = startDate.AddDays(daysDifference);
            TimeSlotRangeInfoDto response = (await _client.GetFromJsonAsync<TimeSlotRangeInfoDto>(MakeTimeSlotRangeUrl(startDate, endDate)))!;
            response.Start.ShouldBe(startDate);
            response.End.ShouldBe(endDate);
            response.TotalDays.ShouldBe(8);
            response.Days.ShouldBe([
                new (startDate, false, false),
                new (startDate.AddDays(1), false, false),
                new (startDate.AddDays(2), false, false),
                new (startDate.AddDays(3), false, true),
                new (startDate.AddDays(4), false, false),
                new (startDate.AddDays(5), false, true),
                new (startDate.AddDays(6), true, false),
                new (startDate.AddDays(7), false, true),
            ]);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task GET_NoDateRange_GetsDefaultDate(bool startDate, bool endDate)
        {
            DateOnly defaultDay = DateOnly.MinValue;
            string uri = MakeTimeSlotRangeUrl(startDate ? defaultDay : null, endDate ? defaultDay : null);
            TimeSlotRangeInfoDto response = (await _client.GetFromJsonAsync<TimeSlotRangeInfoDto>(uri))!;
            response.Start.ShouldBe(defaultDay);
            response.End.ShouldBe(defaultDay);
            response.TotalDays.ShouldBe(1);
            response.Days.ShouldBe([
                new (defaultDay, false, false),
            ]);
        }

        [Fact]
        public async Task GET_InvalidStartDate_Expects404()
        {
            var response = await _client.GetAsync($"TimeSlot/range?startDate=&endDate={DateOnlyToUniversalDate(DateOnly.MaxValue)}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("startDate");
        }

        [Fact]
        public async Task GET_InvalidEndDate_Expects404()
        {
            var response = await _client.GetAsync($"TimeSlot/range?startDate={DateOnlyToUniversalDate(DateOnly.MinValue)}&endDate=");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("endDate");
        }

        [Fact]
        public async Task GET_EndDateBeforeStartDate_Expects404()
        {
            var response = await _client.GetAsync(MakeTimeSlotRangeUrl(DateOnly.MaxValue, DateOnly.MinValue));
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("DateRange");
        }

        [Fact]
        public async Task GET_TimeSlotsByDate_GivesTimeSlots()
        {

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day + 3;


            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"TimeSlot/{year}/{month}/{day}"))!;

            // Assert
            response.ShouldNotBeEmpty();
            response.Count.ShouldBe(3);
            Console.WriteLine(response);
            response.ShouldContain(ts => ts.Start.Equals(TimeSpan.Parse("10:00:00")) && ts.End.Equals(TimeSpan.Parse("11:30:00")));
            response.ShouldContain(ts => ts.Start.Equals(TimeSpan.Parse("13:00:00")) && ts.End.Equals(TimeSpan.Parse("14:00:00")));
            response.ShouldContain(ts => ts.Start.Equals(TimeSpan.Parse("16:30:00")) && ts.End.Equals(TimeSpan.Parse("18:45:00")));
        }



        [Theory]
        [InlineData(8)]
        public async Task GET_TimeSlotsByDate_GivesNoTimeSlots(int daysFromNow)
        {

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = (DateTime.Now.Day + daysFromNow) % 30;


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