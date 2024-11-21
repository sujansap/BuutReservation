using Microsoft.AspNetCore.Components;

namespace Rise.Client.Admins
{
    public partial class GuestDetails : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        protected void NavigateToListPage()
        {
            NavigationManager.NavigateTo($"/admin/guests");
        }
    }
}