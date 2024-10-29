using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.TimeSlots;
using Shouldly;

namespace Rise.Client.Reservations
{
    [TestFixture]
    public class ReservationPageTest : CustomPageTest
    {

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

            ILocator available = Page.Locator("[data-celtype=available]");
            await Expect(available).ToHaveCountAsync(2);

        }

        [Test]
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
            await Page.GotoAsync("/reservations");

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

        // [Test]
        // public async Task CheckRedirectToThisMonthsRange()
        // {
        //     DateTime startDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1).ToDateTime(TimeOnly.MinValue).StartOfWeek(DayOfWeek.Sunday);
        //     DateTime endDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).ToDateTime(TimeOnly.MinValue).StartOfWeek(DayOfWeek.Saturday);
        //     await Page.GotoAsync("/reservations", new PageGotoOptions() {});
        //     Page.Url.ShouldEndWith($"?StartDate={startDate.ToString(universalDateFormat)}&EndDate={endDate.ToString(universalDateFormat)}");
        // }
    }
}