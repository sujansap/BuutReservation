using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Rise.Client;
using MudBlazor.Services;
using Rise.Shared.TimeSlots;
using Rise.Client.TimeSlots;
using Rise.Shared.Products;
using Rise.Client.Products;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddMudPopoverService();

builder.Services.AddHttpClient<ITimeSlotService, TimeSlotService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
});

await builder.Build().RunAsync();
