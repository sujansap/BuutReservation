using System.Threading.Tasks;
using Microsoft.Playwright.MSTest;
using Shouldly;

namespace Rise.Client.Layout
{
    [TestClass]
    public class NavMenuTest : PageTest
    {
        private const int DefaultHeight = 1920;

        [TestMethod]
        [DataRow("nav-brand-logo", "/home", "/reservations")]
        [DataRow("nav-desktop-home", "/home", "/reservations")]
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
            Page.Url.ShouldContain(resultSuffix);
        }

        [TestMethod]
        [DataRow("nav-mobile-home", "home", "/reservations")]
        [DataRow("nav-mobile-about", "about", "")]
        [DataRow("nav-mobile-profile", "profile", "")]
        [DataRow("nav-mobile-reservations", "reservations", "")]
        [DataRow("nav-mobile-book", "book", "")]
        [DataRow("nav-mobile-notifications", "notifications", "")]
        [DataRow("nav-mobile-profile", "profile", "")]
        [DataRow("nav-mobile-notifications", "notifications", "")]
        public async Task Mobile_NavNotifications(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await Page.GotoAsync("https://localhost:5001" + startSuffix);
            string beginUri = Page.Url;
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId(testId).ClickAsync();

            Page.Url.ShouldNotBe(beginUri);
            Page.Url.ShouldContain($"/{resultSuffix}");
        }


        /// <summary>
        /// Test to check if the change language button is visible on the desktop version of the website.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CheckLanguageChangeDesktop()
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await Page.GotoAsync("https://localhost:5001");

            await Page.GetByTestId("culture-selector-desktop").IsVisibleAsync();
        }


        /// <summary>
        /// Test to check if the change language button is visible on the mobile version of the website.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CheckLanguageChangeMobile()
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await Page.GotoAsync("https://localhost:5001");
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId("culture-selector-mobile").IsVisibleAsync();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dutch"></param>
        /// <param name="english"></param>
        /// <returns></returns>
        [TestMethod]
        [DataRow("nav-desktop-home", "HOME", "HOME")]
        [DataRow("nav-desktop-about", "OVER", "ABOUT")]
        [DataRow("nav-desktop-reservations", "RESERVEER", "RESERVE")]
        public async Task ChangeLanguageBetweenLanguagesDesktop(string id, string dutch, string english)
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await Page.GotoAsync("https://localhost:5001/");

            await Page.GetByTestId("culture-selector-desktop").First.ClickAsync();
            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(dutch);
            await Page.GetByTestId("en (US)").ClickAsync();
            await Page.GotoAsync("https://localhost:5001/reservations");
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();

            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(english);

        }


        /// <summary>
        /// Test to check if the language changes between the languages on the mobile version of the website.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dutch"></param>
        /// <param name="english"></param>
        /// <returns></returns>
        [TestMethod]
        [DataRow("nav-mobile-home", "HOME", "HOME")]
        [DataRow("nav-mobile-about", "OVER", "ABOUT")]
        [DataRow("nav-mobile-reservations", "RESERVEER", "RESERVE")]
        public async Task ChangeLanguageBetweenLanguagesMobile(string id, string dutch, string english)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await Page.GotoAsync("https://localhost:5001/");
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId("culture-selector-mobile").First.ClickAsync();
            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(dutch);

            await Page.GetByTestId("en (US)").ClickAsync();

            await Page.GotoAsync("https://localhost:5001/reservations");

            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();

            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(english);

        }
    }
}