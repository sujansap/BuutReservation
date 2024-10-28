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

            // TODO make beter tests for checking date availability
        }

        [TestMethod]
        public async Task HasTimeslotsInTimeSlotList()
        {

            // Arange
            DateOnly today = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            TimeSlotDto[] timeSlotDtos = [
                new()
                {
                    Id = 1,
                    Start = new TimeOnly(9, 0, 0),
                    End = new TimeOnly(12, 0, 0),
                    IsBookedByUser = false
                },
                new()
                {
                    Id = 2,
                    Start = new TimeOnly(12, 0, 0),
                    End = new TimeOnly(15, 0, 0),
                    IsBookedByUser = false
                },
                new()
                {
                    Id = 3,
                    Start = new TimeOnly(15, 0, 0),
                    End = new TimeOnly(18, 0, 0),
                    IsBookedByUser = true
                },
            ];

            // Act
            await Page.RouteAsync("*/**/api/TimeSlot/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(timeSlotDtos)
                });
            });
            await Page.GotoAsync("https://localhost:5001/reservations");

            var dateToFind = today.AddDays(2);
            var day = Page.Locator($"[identifier='{dateToFind.Day}/{dateToFind.Month}/{dateToFind.Year}']");
            await day.ClickAsync();


            var timeSlotList = Page.GetByTestId("time-slot-list");

            var timeSlot1 = timeSlotList.GetByTestId("time-slot-1");
            var timeSlot2 = timeSlotList.GetByTestId("time-slot-2");
            var timeSlot3 = timeSlotList.GetByTestId("time-slot-3");


            // Assert

            timeSlot1.ShouldNotBeNull();
            timeSlot2.ShouldNotBeNull();
            timeSlot3.ShouldNotBeNull();

            // Assert the styles
            await Expect(timeSlot1).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-dark-rgb), 0.1);");
            await Expect(timeSlot2).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-dark-rgb), 0.1);");
            await Expect(timeSlot3).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-primary-rgb), 0.1);");
        }

        [TestMethod]
        public async Task GreyedOutCalendarCellsAreUnclickable()
        {
            // Arrange
            var today = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // Act
            await Page.GotoAsync("https://localhost:5001/reservations");
            var calendarCellToday = Page.GetByText($"{today.Day}");
            await calendarCellToday.ClickAsync();

            // Assert
            var timeSlotList = Page.GetByTestId("time-slot-list");

            var hasContent = await timeSlotList.Locator(":scope > *").CountAsync() > 0;
            Assert.IsFalse(hasContent, "Time slot list should be empty");
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