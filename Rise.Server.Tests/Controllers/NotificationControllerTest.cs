using System;
using System.Net;
using System.Net.Http.Json;
using Rise.Server.Tests.Fixtures;
using Rise.Shared.Notifications;
using Shouldly;

namespace Rise.Server.Tests.Controllers
{
    public class NotificationControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "Notification")

    {
        [Fact]
        public async Task GET_CurrentUser_Notifications_ReturnsOk()
        {
            var response = await _client.GetAsync("me");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GET_CurrentUser_Notifications_GivesNotifications()
        {
            IEnumerable<NotificationDto> response = (await _client.GetFromJsonAsync<IEnumerable<NotificationDto>>("me"))!;
            response.Count().ShouldBe(20);
        }
    }

}

