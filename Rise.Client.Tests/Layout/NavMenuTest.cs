using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Shouldly;

namespace Rise.Client.Layout
{
    [TestClass]
    public class NavMenuTest : PageTest
    {
        [TestMethod]
        // [DataRow(600, 960)]
        [DataRow(960, 1280)]
        [DataRow(1280, 1920)]
        [DataRow(1920, 2560)]
        public async Task Desktop_NavReservations(int width, int height)
        {
            await Page.SetViewportSizeAsync(width, height);
            await Page.GotoAsync("https://localhost:5001");
            string beginUri = Page.Url;
            await Page.GetByTestId("nav-desktop-reservations").ClickAsync();

            Page.Url.ShouldNotBe(beginUri);
            Page.Url.ShouldEndWith("/reservations");
        }
    }
}