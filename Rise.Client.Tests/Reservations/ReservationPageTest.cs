using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

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
        public async Task HasReserveCalendarComponent()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("calendar-reserve").IsVisibleAsync();
        }

        [TestMethod]
        public async Task HasYourReservationsCalendarComponent()
        {
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("calendar-your-reservations").IsVisibleAsync();
        }
    }
}