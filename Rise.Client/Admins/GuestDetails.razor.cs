using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Users;

namespace Rise.Client.Admins
{
    public partial class GuestDetails : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        public required AsyncData<UserDetailDto> AsyncDataRef { get; set; }
        private UserDetailDto? UserDetails { get; set; }

        [Inject]
        public required IUserService UserService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        protected void NavigateToListPage()
        {
            NavigationManager.NavigateTo($"/admin/guests");
        }




        private Task<UserDetailDto> FetchUserDetails()
        {
            return UserService.GetUserDetails(Id);
        }
    }
}