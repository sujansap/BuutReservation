using System;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Pages;

public partial class Authentication
{
    [Parameter] public string? Action { get; set; }
}
