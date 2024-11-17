using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.TimeSlots;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace Rise.Server.Tests.Controllers
{
    public class TimeSlotControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "TimeSlot")
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

            return $"range{joinedQueries}";
        }

        [Fact]
        public async Task GET_ValidDateRange_GivesDates()
        {
            int daysDifference = 7;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endDate = startDate.AddDays(daysDifference);
            TimeSlotRangeInfoDto response = (await _client.GetFromJsonAsync<TimeSlotRangeInfoDto>(MakeTimeSlotRangeUrl(startDate, endDate)))!;
            response.TotalDays.ShouldBe(8);
            response.Days.ShouldBe([
                new(startDate, false, false, false),
                new(startDate.AddDays(1), false, false, false),
                new(startDate.AddDays(2), false, false, false),
                new(startDate.AddDays(3), false, true, false),
                new(startDate.AddDays(4), false, true, false),
                new(startDate.AddDays(5), false, true, false),
                new(startDate.AddDays(6), false, true, false),
                new(startDate.AddDays(7), false, false, false),
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
            response.TotalDays.ShouldBe(1);
            response.Days.ShouldBe([
                new(defaultDay, false, false),
            ]);
        }

        [Fact]
        public async Task GET_InvalidStartDate_Expects404()
        {
            var response = await _client.GetAsync($"range?startDate=&endDate={DateOnlyToUniversalDate(DateOnly.MaxValue)}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("startDate");
        }

        [Fact]
        public async Task GET_InvalidEndDate_Expects404()
        {
            var response = await _client.GetAsync($"range?startDate={DateOnlyToUniversalDate(DateOnly.MinValue)}&endDate=");
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
            // Use the start date of the first upcoming cruise period (index 2 in the array)
            DateTime cruisePeriodStart = DateTime.Now.AddDays(3);
            DateTime threeDaysInAdvance = cruisePeriodStart.AddDays(3);
            int year = threeDaysInAdvance.Year;
            int month = threeDaysInAdvance.Month;
            int day = threeDaysInAdvance.Day;

            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"{year}/{month}/{day}"))!;

            // Assert
            response.ShouldNotBeEmpty();
            response.Count.ShouldBe(3);

            response.ShouldContain(ts => ts.Start.Equals(TimeOnly.Parse("10:00:00")) && ts.End.Equals(TimeOnly.Parse("11:30:00")));
            response.ShouldContain(ts => ts.Start.Equals(TimeOnly.Parse("13:00:00")) && ts.End.Equals(TimeOnly.Parse("14:00:00")));
            response.ShouldContain(ts => ts.Start.Equals(TimeOnly.Parse("16:30:00")) && ts.End.Equals(TimeOnly.Parse("18:45:00")));
        }

        [Theory]
        [InlineData(8)]
        public async Task GET_TimeSlotsByDate_GivesNoTimeSlots(int daysFromNow)
        {
            DateTime daysInAdvance = DateTime.Now.AddDays(daysFromNow);
            int year = daysInAdvance.Year;
            int month = daysInAdvance.Month;
            int day = daysInAdvance.Day;

            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"{year}/{month}/{day}"))!;

            // Assert
            response.ShouldBeEmpty();
            response.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GET_TimeSlotsByDate_InvalidDate_ReturnsBadRequest()
        {

            DateTime tomorrow = DateTime.Now.AddDays(1);
            int year = tomorrow.Year;
            int month = 13;
            int day = tomorrow.Day;


            var response = await _client.GetAsync($"{year}/{month}/{day}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}