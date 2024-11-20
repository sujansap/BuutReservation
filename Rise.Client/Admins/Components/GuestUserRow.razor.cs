using Microsoft.AspNetCore.Components;
using Rise.Shared.Users;

namespace Rise.Client.Admins.Components
{

    public partial class GuestUserRow : ComponentBase
    {
        [Parameter, EditorRequired]
        public required UserDto User { get; set; }


    }
}