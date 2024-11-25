using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Rise.Client;
using MudBlazor.Services;
using Rise.Shared.TimeSlots;
using Rise.Shared.Reservations;
using Rise.Client.TimeSlots;
using Rise.Client.Services;
using MudBlazor;
using System.Globalization;
using Microsoft.JSInterop;
using Serilog.Core;
using Serilog;
using Rise.Shared.Notifications;
using Rise.Client.Notifications;
using Rise.Shared.Users;
using Rise.Client.Admins;
using Rise.Shared.Notifications;
using Rise.Client.Notifications;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var levelSwitch = new LoggingLevelSwitch();
Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
                .WriteTo.BrowserConsole()
                .WriteTo.BrowserHttp($"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: levelSwitch)
                .CreateLogger();

builder.Services.AddLocalization();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.VisibleStateDuration = 8000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;

});
builder.Services.AddMudPopoverService();

builder.Services.AddHttpClient<ITimeSlotService, TimeSlotService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/TimeSlot/");
});

builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Reservation/");
});
builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Notification/");
});
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/User/");
});
builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Notification/");
});

var host = builder.Build();

const string defaultCulture = "nl-BE";

var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");
var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

if (result == null)
{
    await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

Log.Information("Starting up Client in Environment: {Environment}",
                builder.HostEnvironment.Environment);

await host.RunAsync();
