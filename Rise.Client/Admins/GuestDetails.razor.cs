using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Rise.Client.Common;
using Rise.Client.Localization.Admins;
using Rise.Shared.Users;


namespace Rise.Client.Admins
{
    public partial class GuestDetails : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }
        private string name = "John Doe";
        private string email = "johndoe@example.com";
        private string adres = "123 Street, City";
        private string phone = "+32 123 456 789";

        public required AsyncData<UserDetailDto> AsyncDataRef { get; set; }
        private UserDetailDto? UserDetails { get; set; }

        [Inject]
        public required IUserAdminService UserService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        [Inject]
        public required IStringLocalizer<AdminPageResources> AdminPageLocalizer { get; set; }

        private bool isProcessing = false;

        protected void NavigateToListPage()
        {
            NavigationManager.NavigateTo("/admin/guests");
        }

        private async Task ApproveGuest()
        {
            try
            {
                isProcessing = true;
                StateHasChanged();

                await UserService.AddMemberRole(Id);

                Snackbar.Add(AdminPageLocalizer["ApproveSuccess"], Severity.Success);

                NavigateToListPage();
            }
            catch (Exception ex)
            {
                Snackbar.Add(string.Format(AdminPageLocalizer["ApproveFailure"], ex.Message), Severity.Error);
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
            }
        }

        private Task<UserDetailDto> FetchUserDetails()
        {
            return UserService.GetUserDetails(Id);
        }
    }
}