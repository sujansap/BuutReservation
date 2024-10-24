using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Rise.Client;
using Rise.Client.Products;
using Rise.Shared.Products;
using MudBlazor.Services;
using Rise.Shared.TimeSlots;
using Rise.Client.TimeSlots;
using MudBlazor;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
});
builder.Services.AddMudPopoverService();

builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
});

builder.Services.AddHttpClient<ITimeSlotService, TimeSlotService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/TimeSlot/");
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
});

await builder.Build().RunAsync();
