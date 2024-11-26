using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Rise.Client.Layout
{
    public partial class NavMenu
    {
        private bool _drawerOpen = false;
        private bool _notificationPopoverOpen = false;

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        public void BeginLogOut()
        {
            Navigation.NavigateToLogout("authentication/logout");
        }

        private void HandleNotificationButtonClicked()
        {
            _notificationPopoverOpen = !_notificationPopoverOpen;
        }
    }
}
