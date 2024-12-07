using System.Net;
using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.Boats;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admins.Battery
{
    public class AdminBatteryPageTestAdmin : CustomAuthenticatedPageTest
    {
        private const string baseSuffix = "/admin/battery/";

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Administrator);
        }

        private static RouteFulfillOptions FulfillWithOkResponse(RouteFulfillOptions fulfillOptions, object Data)
        {
            fulfillOptions.Status = (int)HttpStatusCode.OK;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize(Data);
            return fulfillOptions;
        }

        private static RouteFulfillOptions FulfillWithNotFoundResponse(RouteFulfillOptions fulfillOptions)
        {
            fulfillOptions.Status = (int)HttpStatusCode.NotFound;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize("Object not found");
            return fulfillOptions;
        }

        private static RouteFulfillOptions FulfillWithBadRequestResponse(RouteFulfillOptions fulfillOptions)
        {
            fulfillOptions.Status = (int)HttpStatusCode.BadRequest;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize("Bad Id");
            return fulfillOptions;
        }

        private async Task MockBatteryDetailsValid(int batteryId, BatteryDto batteryDto, int? delayMs = null)
        {
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                if (delayMs.HasValue)
                {
                    await Task.Delay(delayMs.Value);
                }
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, batteryDto);
                }
                else if (request.Method == "PUT")
                {
                    JsonElement? body = request.PostDataJSON();
                    BatteryDto newBattery = new()
                    {
                        Id = batteryId,
                        Type = body?.GetProperty("type").GetString() ?? "",
                        MentorId = body?.GetProperty("mentorId").GetInt32() ?? 1,
                    };

                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, newBattery);
                }

                await route.FulfillAsync(fulfillOptions);
            });
        }

        // TODO tests admin navmenu
        [Test]
        public async Task ShouldUpdateBatteryDetails()
        {
            int batteryId = 1;
            BatteryDto battery = new() { Id = batteryId, Type = "Lithium", MentorId = 1 };
            BatteryUpdateDto updatedBattery = new() { Type = "Zink", MentorId = 2 };
            await MockBatteryDetailsValid(batteryId, battery, 2000);
            await NavigateToUrl(baseSuffix);

            ILocator placeholder = Page.GetByTestId("battery-details-placeholder");
            await Expect(placeholder).ToBeVisibleAsync();
            ILocator submitButton = Page.GetByTestId("battery-details-submit");
            await Expect(submitButton).ToBeDisabledAsync();

            // Check current data
            // ******************

            ILocator type = Page.GetByTestId("battery-details-type");
            await Expect(type).ToBeVisibleAsync();
            await Expect(type).ToHaveValueAsync(battery.Type);

            ILocator mentorId = Page.GetByTestId("battery-details-mentor-id");
            await Expect(mentorId).ToBeVisibleAsync();
            await Expect(mentorId).ToHaveValueAsync(battery.MentorId.ToString());

            // Update data
            // ******************

            await mentorId.ClickAsync();
            await mentorId.FillAsync(updatedBattery.MentorId.ToString());
            await Expect(mentorId).ToHaveValueAsync(updatedBattery.MentorId.ToString());

            await submitButton.ClickAsync();

            await Expect(placeholder).ToBeVisibleAsync();

            // Verify result
            // ************
            await Expect(type).ToHaveValueAsync(battery.Type);

            await Expect(mentorId).ToHaveValueAsync(updatedBattery.MentorId.ToString());

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).Not.ToBeVisibleAsync();

            ILocator success = Page.GetByTestId("battery-details-success");
            await Expect(success).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldNotFindBatteryId()
        {
            int batteryId = 666;
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithNotFoundResponse(fulfillOptions);
                }

                await route.FulfillAsync(fulfillOptions);
            });

            await NavigateToUrl(baseSuffix);

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldNotAllowBadBatteryId()
        {
            int batteryId = -1;
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithBadRequestResponse(fulfillOptions);
                }

                await route.FulfillAsync(fulfillOptions);
            });

            await NavigateToUrl(baseSuffix);

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldNotFindMentorId()
        {
            int batteryId = 1;
            BatteryDto battery = new() { Id = batteryId, Type = "Lithium", MentorId = 1 };
            BatteryUpdateDto updatedBattery = new() { Type = "Zink", MentorId = 666 };
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, battery);
                }
                else if (request.Method == "PUT")
                {
                    fulfillOptions = FulfillWithNotFoundResponse(fulfillOptions);
                }

                await route.FulfillAsync(fulfillOptions);
            });

            await NavigateToUrl(baseSuffix);

            ILocator type = Page.GetByTestId("battery-details-type");
            await Expect(type).ToBeVisibleAsync();
            await Expect(type).ToHaveValueAsync(battery.Type);

            ILocator mentorId = Page.GetByTestId("battery-details-mentor-id");
            await Expect(mentorId).ToBeVisibleAsync();
            await Expect(mentorId).ToHaveValueAsync(battery.MentorId.ToString());

            ILocator submitButton = Page.GetByTestId("battery-details-submit");
            await submitButton.ClickAsync();

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldNotAllowBadMentorId()
        {
            int batteryId = 1;
            BatteryDto battery = new() { Id = batteryId, Type = "Lithium", MentorId = 1 };
            BatteryUpdateDto updatedBattery = new() { Type = "Zink", MentorId = -1 };
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, battery);
                }
                else if (request.Method == "PUT")
                {
                    fulfillOptions = FulfillWithBadRequestResponse(fulfillOptions);
                }

                await route.FulfillAsync(fulfillOptions);
            });

            await NavigateToUrl(baseSuffix);

            ILocator type = Page.GetByTestId("battery-details-type");
            await Expect(type).ToBeVisibleAsync();
            await Expect(type).ToHaveValueAsync(battery.Type);

            ILocator mentorId = Page.GetByTestId("battery-details-mentor-id");
            await Expect(mentorId).ToBeVisibleAsync();
            await Expect(mentorId).ToHaveValueAsync(battery.MentorId.ToString());

            ILocator submitButton = Page.GetByTestId("battery-details-submit");
            await submitButton.ClickAsync();

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).ToBeVisibleAsync();
        }
    }
}
