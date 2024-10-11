using System;

namespace Rise.Client.Layout
{
    public partial class NavMenu
    {
        private bool _drawerOpen = false;
        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
