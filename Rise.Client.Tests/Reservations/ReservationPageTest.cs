using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiffEngine;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using MudBlazor.Extensions;
using Rise.Shared.TimeSlots;
using Shouldly;

namespace Rise.Client.Reservations
{
    [TestClass]
    public class ReservationPageTest : PageTest
    {
        private const string universalDateFormat = "yyyy-MM-dd";

        [TestMethod]
        public async Task HasTabs()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }

        [TestMethod]
        public async Task HasCustomCalendarReserveComponent()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("custom-calendar-reserve").IsVisibleAsync();
        }

        [TestMethod]
        public async Task HasCorrectAmountOfDaysInCalendar()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");

            // Wait for the page to load
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Use a simple class selector to find all calendar day cells
            var days = await Page.Locator(".mud-cal-month-cell").AllAsync();

            // Assert the correct number of days
            Assert.AreEqual(35, days.Count, "The calendar should have 35 day elements (5 weeks * 7 days)");
        }

        [TestMethod]
        public async Task HasLegendComponent()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("custom-calendar-legend").IsVisibleAsync();
        }

        [TestMethod]
        public async Task HasYourReservationsCalendarComponent()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("calendar-your-reservations").IsVisibleAsync();
        }

        [TestMethod]
        public async Task HasUnexpectedError()
        {
            await Page.RouteAsync("*/**/api/TimeSlot/range/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 400,
                    ContentType = "text/plain",
                    Body = "Bad argument!"
                });
            });
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("error-message").IsVisibleAsync();
        }

        [TestMethod]
        public async Task CheckDateTypes()
        {
            int totalDays = 4;
            DateOnly startDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateOnly endDate = startDate.AddDays(totalDays);
            TimeSlotRangeInfoDto dto = new(
                Start: startDate,
                End: startDate.AddDays(totalDays),
                TotalDays: totalDays,
                Days: [
                    new(startDate, false, false),
                    new(startDate.AddDays(1), true, false),
                    new(startDate.AddDays(2), false, true),
                    new(startDate.AddDays(3), true, true),
                ]
            );
            await Page.RouteAsync("*/**/api/TimeSlot/range/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(dto)
                });
            });
            await Page.GotoAsync("https://localhost:5001/reservations");
            var locator = Page.Locator($"[identifier={startDate}]");
            var child = locator.GetByTestId("custom-calendar-day");
            child.ShouldNotBeNull();
        }

        // [TestMethod]
        // public async Task CheckRedirectToThisMonthsRange()
        // {
        //     DateTime startDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1).ToDateTime(TimeOnly.MinValue).StartOfWeek(DayOfWeek.Sunday);
        //     DateTime endDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).ToDateTime(TimeOnly.MinValue).StartOfWeek(DayOfWeek.Saturday);
        //     await Page.GotoAsync("https://localhost:5001/reservations", new PageGotoOptions() {});
        //     Page.Url.ShouldEndWith($"?StartDate={startDate.ToString(universalDateFormat)}&EndDate={endDate.ToString(universalDateFormat)}");
        // }
    }
}