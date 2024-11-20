using System;
using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Users;

namespace Rise.Client.Admins
{
    public partial class Guests : ComponentBase
    {

        public required AsyncData<IEnumerable<UserDto>> AsyncDataRef { get; set; }

        private IEnumerable<UserDto> Users { get; set; } = [];

        [Inject]
        public required IUserService UserService { get; set; }

        private Task<IEnumerable<UserDto>> FetchUsers()
        {
            return UserService.GetGuestUsers();
        }

        private async Task RefreshUserList()
        {
            await AsyncDataRef.FetchData();
        }
    }
}