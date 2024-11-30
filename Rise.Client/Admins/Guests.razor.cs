using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
using Rise.Shared.Users;

namespace Rise.Client.Admins
{
    public partial class Guests : ComponentBase
    {
        public required AsyncData<UsersPagination<UserDto>> AsyncDataRef { get; set; }
        private UsersPagination<UserDto> Users { get; set; } = new();

        private int CurrentPage = 1;
        private const int PageSize = 2;
        private int TotalPages => (int)Math.Ceiling(Users.TotalCount / (double)PageSize);

        [Inject]
        public required IUserService UserService { get; set; }

        private async Task<UsersPagination<UserDto>> FetchUsers()
        {
            return await UserService.GetUsersByRole(UserRole.Guest, CurrentPage, PageSize);
        }

        private async Task OnPageChanged(int page)
        {
            CurrentPage = page;
            await AsyncDataRef.FetchData();
        }


    }
}
