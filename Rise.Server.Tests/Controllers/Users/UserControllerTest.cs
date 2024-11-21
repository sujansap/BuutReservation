using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.Users;
using System.Net.Http.Json;
using System.Net;

namespace Rise.Server.Tests.Controllers.Users
{
    public class UserControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "User")
    {
        [Fact]
        public async Task GET_GuestUsers_ReturnsGuestList()
        {
            var response = await _client.GetFromJsonAsync<List<UserDto>>("guests");

            response.ShouldNotBeNull();
            response.ShouldNotBeEmpty();

        }

        [Fact]
        public async Task GET_GuestUsers_ContainsExpectedFields()
        {
            var response = await _client.GetFromJsonAsync<List<UserDto>>("guests");


            response.ShouldNotBeNull();
            var firstUser = response.First();

            firstUser.Id.ShouldNotBe(default);
            firstUser.FamilyName.ShouldNotBeNull();

        }

        [Fact]
        public async Task GET_GuestUsers_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("guests");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Fact]
        public async Task GET_GuestUsers_ValidatesResponseFormat()
        {

            var response = await _client.GetFromJsonAsync<List<UserDto>>("guests");


            response.ShouldNotBeNull();
            foreach (var user in response)
            {
                user.FamilyName.ShouldNotBeNullOrWhiteSpace();
            }
        }
    }
}