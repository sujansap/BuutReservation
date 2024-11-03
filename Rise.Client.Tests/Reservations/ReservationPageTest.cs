using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.TimeSlots;
using Shouldly;

namespace Rise.Client.Reservations
{
    [TestFixture]
    public class ReservationPageTest : CustomPageTest
    {

        [SetUp]
        public async Task Setup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "playwright-traces",
                $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
            )
            });
        }

        private const string universalDateFormat = "yyyy-MM-dd";

        private static string DateToCalendarIdentifier(DateOnly date)
        {
            return $"[identifier='{date:d/MM/yyyy}']";
        }

        private async Task MockAvailableDays()
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
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(dto)
                });
            });
            await Page.GotoAsync("/reservations");
        }

        private async Task MockTimeSlotAndSelectOnCalendar()
        {
            // Arange
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            TimeSlotRangeInfoDto timeRange = new(
                TotalDays: 1,
                Days: [
                    new(today, false, true),
                ]
            );


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
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(timeRange)
                });
            });
            await Page.RouteAsync("*/**/api/TimeSlot/*/*/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(timeSlotDtos)
                });
            });

            await Page.GotoAsync("/reservations");

            ILocator day = Page.Locator(DateToCalendarIdentifier(today));
            await day.ClickAsync();
        }

        [Test]
        public async Task HasTabs()
        {
            await Page.GotoAsync("/reservations");
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }

        [Test]
        public async Task HasCustomCalendarReserveComponent()
        {
            await Page.GotoAsync("/reservations");
            await Page.GetByTestId("custom-calendar-reserve").IsVisibleAsync();
        }

        [Test]
        public async Task HasCorrectAmountOfDaysInCalendar()
        {
            await Page.GotoAsync("/reservations");

            ILocator daysLocator = Page.GetByTestId("calendar-cel");

            // Assert the correct number of days
            await Expect(daysLocator).ToHaveCountAsync(35);
        }

        [Test]
        public async Task HasLegendComponent()
        {
            await Page.GotoAsync("/reservations");
            await Page.GetByTestId("custom-calendar-legend").IsVisibleAsync();
        }

        [Test]
        public async Task HasYourReservationsCalendarComponent()
        {
            await Page.GotoAsync("/reservations");
            await Page.GetByTestId("calendar-your-reservations").IsVisibleAsync();
        }

        [Test]
        public async Task HasUnexpectedError()
        {
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 400,
                    ContentType = "text/plain",
                    Body = "Bad argument!"
                });
            });
            await Page.GotoAsync("/reservations");
            await Page.GetByTestId("error-message").IsVisibleAsync();
        }

        [Test]
        public async Task ContainCalendarDataDates()
        {
            await MockAvailableDays();
            await Page.GotoAsync("/reservations");

            ILocator available = Page.Locator("[data-celtype=available]");
            await Expect(available).ToHaveCountAsync(2, new LocatorAssertionsToHaveCountOptions() { Timeout = 8000 });
        }

        [Test]
        public async Task ContainNoAvailableDateDates()
        {
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(new List<object>())
                });
            });
            await Page.GotoAsync("/reservations");

            ILocator booked = Page.Locator("[data-celtype=fully-booked]");
            await Expect(booked).ToHaveCountAsync(35, new LocatorAssertionsToHaveCountOptions()
            {
                Timeout = 8000
            });

        }

        //  TODO make tests for no timeslots found and loading

        [Test]
        public async Task HasTimeSlotsInTimeSlotList()
        {
            await MockTimeSlotAndSelectOnCalendar();

            ILocator timeSlotList = Page.GetByTestId("time-slot-list");

            ILocator timeSlot1 = timeSlotList.GetByTestId("time-slot-1");
            ILocator timeSlot2 = timeSlotList.GetByTestId("time-slot-2");
            ILocator timeSlot3 = timeSlotList.GetByTestId("time-slot-3");


            // Assert

            await Expect(timeSlot1).ToHaveCountAsync(1);
            await Expect(timeSlot2).ToHaveCountAsync(1);
            await Expect(timeSlot3).ToHaveCountAsync(1);

            // Assert the styles
            // TODO assert type of time slots
            // await Expect(timeSlot1).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-dark-rgb), 0.1);");
            // await Expect(timeSlot2).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-dark-rgb), 0.1);");
            // await Expect(timeSlot3).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-primary-rgb), 0.1);");
        }

        [Test]
        public async Task ShouldNotBeAbleToGoBackToPreviousMonthFromCurrentUsingButtons()
        {

            await Page.GotoAsync("/reservations");
            ILocator prev = Page.GetByTestId("calendar-previous");
            (await prev.IsDisabledAsync()).ShouldBeTrue();
        }

        [Test]
        public async Task ShouldNotBeAbleToGoBackToPreviousMonthFromCurrentUsingPicker()
        {

            await Page.GotoAsync("/reservations");

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

        [Test]
        public async Task ShouldBeAbleToGoBackToPreviousMonthFromNextMonthUsingButtons()
        {
            await Page.GotoAsync("/reservations");

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

        [Test]
        public async Task ShouldBeAbleToGoBackToPreviousMonthFromNextMonthUsingDatePicker()
        {
            await Page.GotoAsync("/reservations");

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

        [Test]
        public async Task ShouldRedirectToThisMonthsCurrentDateWhenNoCurrentDate()
        {
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await Page.GotoAsync("/reservations");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={currentDate}')", options: new PageWaitForFunctionOptions() { Timeout = 8000 });
            Page.Url.ShouldContain($"CurrentDate={currentDate}");
        }

        [Test]
        public async Task ShouldRedirectToThisMonthsCurrentDateWhenToEarlyDate()
        {
            string toEarlyDate = DateTime.Today.AddDays(-1).ToString(universalDateFormat);
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await Page.GotoAsync($"/reservations?CurrentDate={toEarlyDate}");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={currentDate}')", options: new PageWaitForFunctionOptions() { Timeout = 8000 });
            Page.Url.ShouldContain($"CurrentDate={currentDate}");
        }

        [Test]
        public async Task ShouldRedirectToGivenCurrentDate()
        {

            DateTime plusOneMonthDate = DateTime.Today.AddMonths(1);
            string plusOneMonthDateFormatted = plusOneMonthDate.ToString(universalDateFormat);
            await Page.GotoAsync($"/reservations?CurrentDate={plusOneMonthDateFormatted}");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={plusOneMonthDateFormatted}')", options: new PageWaitForFunctionOptions() { Timeout = 8000 });
            Page.Url.ShouldContain($"CurrentDate={plusOneMonthDateFormatted}");

            ILocator date = Page.Locator(DateToCalendarIdentifier(DateOnly.FromDateTime(plusOneMonthDate)));
            date.ShouldNotBeNull();
        }

        [Test]
        public async Task ShouldChangeCurrentDateWhenGoingToNextMonth()
        {
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await Page.GotoAsync("/reservations");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={currentDate}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });

            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            string nextMonthDate = DateTime.Today.AddMonths(1).ToString(universalDateFormat);
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('?CurrentDate={nextMonthDate}')", options: new PageWaitForFunctionOptions() { Timeout = 5000 });
            Page.Url.ShouldContain($"CurrentDate={nextMonthDate}");
        }


        [Test]
        public async Task FullyBookedDaysAreUnclickable()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            await Page.GotoAsync("/reservations");

            ILocator timeSlotList = Page.GetByTestId("time-slot-list");
            await Expect(timeSlotList).ToHaveCountAsync(0);

            ILocator calendarCellToday = Page.Locator(DateToCalendarIdentifier(today));
            await calendarCellToday.ClickAsync();

            await Expect(timeSlotList).ToHaveCountAsync(0);
        }

        [Test]
        public async Task SelectingFullyBookedDayShouldRemoveExistingSelectedDay()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            await MockTimeSlotAndSelectOnCalendar();

            ILocator timeSlotList = Page.GetByTestId("time-slot-list");
            await Expect(timeSlotList).ToHaveCountAsync(1);

            DateOnly day = new(today.Year, today.Month, 1);
            if (today.Day == 1) day = day.AddDays(1);

            ILocator calendarCellToday = Page.Locator(DateToCalendarIdentifier(day));
            await calendarCellToday.ClickAsync();

            await Expect(timeSlotList).ToHaveCountAsync(0);
        }

        [Test]
        public async Task ReservationDialogShouldCloseWhenCloseButtonIsClicked()
        {
            //Arrange
            await OpenCreateReservationDialog();

            ILocator dialog = Page.GetByTestId("reservation-dialog");
            ILocator closeButton = Page.GetByTestId("dialog-cancel-button");
            await closeButton.ClickAsync(new() { Timeout = 8000 });

            await Expect(dialog).Not.ToBeVisibleAsync();
        }

        [Test]
        public async Task ReservationDialogShouldProceedWhenCreateButtonIsClicked()
        {
            //Arrange
            await OpenCreateReservationDialog();

            // Act
            // ILocator dialog = Page.GetByTestId("reservation-dialog");
            ILocator closeButton = Page.GetByTestId("dialog-create-button");
            await closeButton.ClickAsync(new() { Timeout = 8000 });

            // Assert
            ILocator dialogPaymentContent = Page.GetByTestId("dialog-payment-content");
            await Expect(dialogPaymentContent).ToBeVisibleAsync();

            // TODO mock post to server
            // wait for the payment to go trough
            await Task.Delay(4000);

            ILocator dialogSuccessContent = Page.GetByTestId("dialog-success-content");
            await Expect(dialogSuccessContent).ToBeVisibleAsync();
        }

        private async Task OpenCreateReservationDialog()
        {
            await MockTimeSlotAndSelectOnCalendar();
            ILocator timeSlot1 = Page.GetByTestId("time-slot-1");
            await timeSlot1.ClickAsync();
            await Page.WaitForSelectorAsync("[data-testid='reservation-dialog']", new() { State = WaitForSelectorState.Visible });
        }
    }
}