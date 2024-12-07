using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Rise.Shared.Notifications;

namespace Rise.Client.Layout
{
    public partial class NavMenu
    {
        private bool _drawerOpen = false;
        private bool _notificationPopoverOpen = false;
        private int _unreadNotificationCount = 0;


        [Inject]
        public required NavigationManager Navigation { get; set; }
        [Inject]
        public required INotificationService NotificationService { get; set; }
        [Inject]
        public required AuthenticationStateProvider AuthStateProvider { get; set; }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState? authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user?.Identity?.IsAuthenticated ?? false)
                await UpdateNotificationCount();
        }

        private async Task UpdateNotificationCount()
        {
            _unreadNotificationCount = await NotificationService.GetUnreadNotificationCount();
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
