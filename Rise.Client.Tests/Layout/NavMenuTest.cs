using System.Text.Json;
using Microsoft.Playwright;
using Rise.Client.Tests.Notifications;
using Rise.Shared.Notifications;
using Shouldly;

namespace Rise.Client.Tests.Layout
{
    [TestFixture]
    public class NavMenuTest : CustomPageTest
    {
        private const int DefaultHeight = 1920;


        public static readonly List<NotificationDto> Notifications = NotificationPageTest.Notifications;
        private async Task MockHTTPRequests()
        {
            await Page.RouteAsync("*/**/api/Notification/me", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(Notifications)
                });
            });

            await Page.RouteAsync("*/**/api/Notification/me?*", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(Notifications.Take(3))
                });
            });
        }

        [Test]
        [TestCase("nav-brand-logo", "/home", "/reservations")]
        [TestCase("nav-desktop-home", "/home", "/reservations")]
        [TestCase("nav-desktop-about", "/about", "")]
        [TestCase("nav-desktop-reservations", "/reservations", "")]
        [TestCase("nav-desktop-book", "/book", "")]
        [TestCase("nav-desktop-profile", "/profile", "")]
        public async Task Desktop_NavMenu(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await InitNavigationToUrl(startSuffix);
            string beginUri = Page.Url;
            await Page.GetByTestId(testId).ClickAsync();

            Page.Url.ShouldNotBe(beginUri);
            Page.Url.ShouldContain(resultSuffix);
        }

        [Test]
        public async Task Desktop_NotificationsPopover_ToBeVisible()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await InitNavigationToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");

            await Expect(notificationButton).ToBeVisibleAsync();
            await notificationButton.ClickAsync();

            ILocator popover = Page.GetByTestId("notifications-popover");
            await Expect(popover).ToBeVisibleAsync();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task Desktop_NotificationsPopover_ToHaveNotifications(int id)
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await InitNavigationToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");
            await notificationButton.ClickAsync();

            ILocator popoverList = Page.GetByTestId("notifications-popover-list");
            await Expect(popoverList).ToBeVisibleAsync();

            ILocator notification = Page.GetByTestId($"notification-{id}");
            await Expect(notification).ToBeVisibleAsync();
        }

        [Test]
        public async Task Desktop_NotificationsPopover_Button_ToBeVisible()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await InitNavigationToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");
            await notificationButton.ClickAsync();

            ILocator popoverButton = Page.GetByTestId("notifications-popover-button");
            await Expect(popoverButton).ToBeVisibleAsync();
        }

        [Test]
        public async Task Desktop_NotificationsPopover_Button_ToNavigate()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await InitNavigationToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");
            await notificationButton.ClickAsync();

            ILocator popoverButton = Page.GetByTestId("notifications-popover-button");
            await popoverButton.ClickAsync();

            Page.Url.ShouldContain("/notifications");
        }

        [Test]
        [TestCase("nav-mobile-home", "home", "/reservations")]
        [TestCase("nav-mobile-about", "about", "")]
        [TestCase("nav-mobile-profile", "profile", "")]
        [TestCase("nav-mobile-reservations", "reservations", "")]
        [TestCase("nav-mobile-book", "book", "")]
        [TestCase("nav-mobile-notifications", "notifications", "")]
        [TestCase("nav-mobile-profile", "profile", "")]
        [TestCase("nav-mobile-notifications", "notifications", "")]
        [TestCase("nav-admin-guests", "admin", "")]
        public async Task Mobile_NavNotifications(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await InitNavigationToUrl(startSuffix);
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
        [Test]
        public async Task CheckLanguageChangeDesktop()
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await InitNavigationToUrl("/");

            await Page.GetByTestId("culture-selector-desktop").IsVisibleAsync();
        }


        /// <summary>
        /// Test to check if the change language button is visible on the mobile version of the website.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CheckLanguageChangeMobile()
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await InitNavigationToUrl("/");
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
        [Test]
        [TestCase("nav-desktop-home", "HOME", "HOME")]
        [TestCase("nav-desktop-about", "OVER", "ABOUT")]
        [TestCase("nav-desktop-reservations", "RESERVEER", "RESERVE")]
        public async Task ChangeLanguageBetweenLanguagesDesktop(string id, string dutch, string english)
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await InitNavigationToUrl("/");

            await Page.GetByTestId("culture-selector-desktop").First.ClickAsync();
            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(dutch);
            await Page.GetByTestId("en (US)").ClickAsync();
            await Page.GotoAsync("/reservations");
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
        [Test]
        [TestCase("nav-mobile-home", "HOME", "HOME")]
        [TestCase("nav-mobile-about", "OVER", "ABOUT")]
        [TestCase("nav-mobile-reservations", "RESERVEER", "RESERVE")]
        public async Task ChangeLanguageBetweenLanguagesMobile(string id, string dutch, string english)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await InitNavigationToUrl("/");
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId("culture-selector-mobile").First.ClickAsync();
            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(dutch);

            await Page.GetByTestId("en (US)").ClickAsync();

            await Page.GotoAsync("/reservations");

            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();

            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            (await Page.GetByTestId(id).TextContentAsync()).ShouldBe(english);

        }


        /// <summary>
        /// Test to check if the admin dashboard is visible on the desktop version of the website.
        /// <returns></returns>
        [Test]
        public async Task CheckAdminDashBoardDesktop()
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await InitNavigationToUrl("/admin");

            await Page.GetByTestId("nav-desktop-admin").IsVisibleAsync();
        }

    }
}