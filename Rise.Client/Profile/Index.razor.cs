using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Users;
using static Rise.Shared.Users.UserProfileDto;

namespace Rise.Client.Profile;


public partial class Index : ComponentBase
{
    [Inject]
    public required ISnackbar SnackbarService { get; set; }
}
