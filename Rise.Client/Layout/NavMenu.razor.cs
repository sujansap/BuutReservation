using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Rise.Client.Layout
{
    public partial class NavMenu
    {
        private bool _drawerOpen = false;
        private bool _notificationPopoverOpen = false;

        [Inject]
        public required NavigationManager Navigation { get; set; }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void HandleNotificationButtonClicked()
        {
            _notificationPopoverOpen = !_notificationPopoverOpen;
        }

        public void BeginLogOut()
        {
            Navigation.NavigateToLogout("authentication/logout");
        }
    }
}
