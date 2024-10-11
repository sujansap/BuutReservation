
using MudBlazor.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Linq;
using System;

namespace Rise.Client.Layout
{
    public class NavMenuShould : TestContext
    {
        private NavigationManager _navigationManager;

        public NavMenuShould()
        {
            Services.AddMudServices();
            JSInterop.Mode = JSRuntimeMode.Loose;
            _navigationManager = Services.GetRequiredService<NavigationManager>();
        }

        [Fact]
        public async Task NavigateToReservationsPageWhenClicked()
        {
            // Arrange
            var navMenu = RenderComponent<NavMenu>();
            var initialUrl = _navigationManager.Uri;

            // Act
            var reservationLink = navMenu.FindComponents<MudNavLink>()
                .FirstOrDefault(x => x.Instance.Href == "reservations");
            Assert.NotNull(reservationLink); // Ensure we found the link
            await reservationLink.Find("a").ClickAsync(new MouseEventArgs());

            // Assert
            Assert.NotEqual(initialUrl, _navigationManager.Uri);
            Assert.EndsWith("/reservations", _navigationManager.Uri);
        }

        [Fact]
        public async Task NavigateToReservationsPageWhenClickedOnMobile()
        {
            // Arrange
            var navMenu = RenderComponent<NavMenu>();
            var initialUrl = _navigationManager.Uri;

            // Open the drawer
            var menuButton = navMenu.Find("button.mud-icon-button");
            await menuButton.ClickAsync(new MouseEventArgs());

            // Wait for the drawer to open
            await Task.Delay(500); // Give some time for the drawer to open

            // Act
            var reservationLink = navMenu.FindComponents<MudNavLink>()
                .FirstOrDefault(x => x.Instance.Href == "reservations");
            Assert.NotNull(reservationLink); // Ensure we found the link
            await reservationLink.Find("a").ClickAsync(new MouseEventArgs());

            // Assert
            Assert.NotEqual(initialUrl, _navigationManager.Uri);
            Assert.EndsWith("/reservations", _navigationManager.Uri);
        }
    }
}