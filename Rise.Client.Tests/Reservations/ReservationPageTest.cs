using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
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

            var day = Page.GetByText($"{today.Day + 2}");
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
        public async Task ShouldNotBeAbleToGoBackToPreviousMonthFromCurrentUsingButtons()
        {

            await Page.GotoAsync("https://localhost:5001/reservations");
            ILocator prev = Page.GetByTestId("calendar-previous");
            (await prev.IsDisabledAsync()).ShouldBeTrue();
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToGoBackToPreviousMonthFromCurrentUsingPicker()
        {

            await Page.GotoAsync("https://localhost:5001/reservations");

            ILocator monthPicker = Page.Locator(".mud-picker-input-button");
            await monthPicker.ClickAsync();

            int currentMonth = DateTime.Now.Month;
            ILocator monthPickerCollapsed = Page.GetByTestId("calendar-datepicker");

            if (currentMonth == 1)
            {
                await monthPickerCollapsed.GetByLabel($"Previous year ({DateTime.Today.AddYears(-1).Year})").ClickAsync();
                currentMonth = 13;
            }

            ILocator previousMonth = monthPickerCollapsed.Locator(".mud-picker-month").Nth(currentMonth - 2);
            (await previousMonth.IsDisabledAsync()).ShouldBeTrue();
        }

        [TestMethod]
        public async Task ShouldBeAbleToGoBackToPreviousMonthFromNextMonthUsingButtons()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");

            ILocator monthPicker = Page.Locator(".mud-picker-input-button");
            string startMonthText = await monthPicker.InnerTextAsync();

            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            string nextMonthText = await monthPicker.InnerTextAsync();
            nextMonthText.ShouldNotBe(startMonthText);

            ILocator prev = Page.GetByTestId("calendar-previous");
            (await prev.IsDisabledAsync()).ShouldBeFalse();
            await prev.ClickAsync();

            string currentMonthText = await monthPicker.InnerTextAsync();
            currentMonthText.ShouldBe(startMonthText);
        }

        [TestMethod]
        public async Task ShouldBeAbleToGoBackToPreviousMonthFromNextMonthUsingDatePicker()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");

            ILocator monthPicker = Page.Locator(".mud-picker-input-button");
            string startMonthText = await monthPicker.InnerTextAsync();

            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            string nextMonthText = await monthPicker.InnerTextAsync();
            nextMonthText.ShouldNotBe(startMonthText);

            await monthPicker.ClickAsync();

            int nextMonth = (DateTime.Now.Month + 1) % 12;
            ILocator monthPickerCollapsed = Page.GetByTestId("calendar-datepicker");

            if (nextMonth == 1)
            {
                await monthPickerCollapsed.GetByLabel($"Previous year ({DateTime.Today.Year})").ClickAsync();
                nextMonth = 13;
            }

            ILocator previousMonth = monthPickerCollapsed.Locator(".mud-picker-month").Nth(nextMonth - 2);
            (await previousMonth.IsDisabledAsync()).ShouldBeFalse();
            await previousMonth.ClickAsync();

            string currentMonthText = await monthPicker.InnerTextAsync();
            currentMonthText.ShouldBe(startMonthText);
        }

        [TestMethod]
        public async Task ShouldRedirectToThisMonthsCurrentDateWhenNoCurrentDate()
        {
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={currentDate}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });
            Page.Url.ShouldContain($"CurrentDate={currentDate}");
        }

        [TestMethod]
        public async Task ShouldRedirectToThisMonthsCurrentDateWhenToEarlyDate()
        {
            string toEarlyDate = DateTime.Today.AddDays(-1).ToString(universalDateFormat);
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await Page.GotoAsync($"https://localhost:5001/reservations?CurrentDate={toEarlyDate}");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={currentDate}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });
            Page.Url.ShouldContain($"CurrentDate={currentDate}");
        }

        [TestMethod]
        public async Task ShouldRedirectToGivenCurrentDate()
        {

            DateTime plusOneMonthDate = DateTime.Today.AddMonths(1);
            string plusOneMonthDateFormatted = plusOneMonthDate.ToString(universalDateFormat);
            await Page.GotoAsync($"https://localhost:5001/reservations?CurrentDate={plusOneMonthDateFormatted}");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={plusOneMonthDateFormatted}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });
            Page.Url.ShouldContain($"CurrentDate={plusOneMonthDateFormatted}");

            ILocator date = Page.Locator($"[identifier={DateOnly.FromDateTime(plusOneMonthDate)}]");
            date.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task ShouldChangeCurrentDateWhenGoingToNextMonth()
        {
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={currentDate}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });

            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            string nextMonthDate = DateTime.Today.AddMonths(1).ToString(universalDateFormat);
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={nextMonthDate}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });
            Page.Url.ShouldContain($"CurrentDate={nextMonthDate}");
        }
    }
}