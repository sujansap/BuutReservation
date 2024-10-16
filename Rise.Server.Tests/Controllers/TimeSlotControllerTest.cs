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
        private static string MakeTimeSlotRangeUrl(DateOnly? startDay, DateOnly? endDay)
        {
            List<string> queries = [];
            if (startDay is not null) queries.Add($"startDay={DateOnlyToUniversalDate((DateOnly)startDay)}");
            if (endDay is not null) queries.Add($"endDay={DateOnlyToUniversalDate((DateOnly)endDay)}");

            string joinedQueries = queries.Count == 0 ? "" : ("?" + string.Join("&", queries));

            return $"TimeSlot/range{joinedQueries}";
        }

        [Fact]
        public async Task GET_ValidDateRange_GivesDates()
        {
            int daysDifference = 7;
            DateOnly startDay = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endDay = startDay.AddDays(daysDifference);
            TimeSlotRangeInfoDto response = (await _client.GetFromJsonAsync<TimeSlotRangeInfoDto>(MakeTimeSlotRangeUrl(startDay, endDay)))!;
            response.Start.ShouldBe(startDay);
            response.End.ShouldBe(endDay);
            response.TotalDays.ShouldBe(8);
            response.Days.ShouldBe([
                new (startDay, false, false),
                new (startDay.AddDays(1), false, false),
                new (startDay.AddDays(2), false, false),
                new (startDay.AddDays(3), false, true),
                new (startDay.AddDays(4), false, true),
                new (startDay.AddDays(5), false, true),
                new (startDay.AddDays(6), true, false),
                new (startDay.AddDays(7), false, false),
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
            var response = await _client.GetAsync($"TimeSlot/range?startDay=&endDay={DateOnlyToUniversalDate(DateOnly.MaxValue)}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("startDay");
        }

        [Fact]
        public async Task GET_InvalidEndDate_Expects404()
        {
            var response = await _client.GetAsync($"TimeSlot/range?startDay={DateOnlyToUniversalDate(DateOnly.MinValue)}&endDay=");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("endDay");
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
    }
}