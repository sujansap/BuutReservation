using System;

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

        private void HandleNotificationButtonClicked()
        {
            _notificationPopoverOpen = !_notificationPopoverOpen;
        }

    }
}
