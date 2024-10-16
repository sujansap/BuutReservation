using System.Reflection.Metadata;
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
        private const int DefaultHeight = 1920;

        [TestMethod]
        [DataRow("nav-brand-logo", "/", "/reservations")]
        [DataRow("nav-desktop-about", "/about", "")]
        [DataRow("nav-desktop-reservations", "/reservations", "")]
        [DataRow("nav-desktop-book", "/book", "")]
        [DataRow("nav-desktop-profile", "/profile", "")]
        [DataRow("nav-desktop-notifications", "/notifications", "")]
        public async Task Desktop_NavMenu(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await Page.GotoAsync("https://localhost:5001" + startSuffix);
            string beginUri = Page.Url;
            await Page.GetByTestId(testId).ClickAsync();

            Page.Url.ShouldNotBe(beginUri);
            Page.Url.ShouldEndWith(resultSuffix);
        }
    }
}