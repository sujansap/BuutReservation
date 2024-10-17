using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Shouldly;

namespace Rise.Client.Reservations
{
    [TestClass]
    public class ReservationPageTest : PageTest
    {
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
    }
}