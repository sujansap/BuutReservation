using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.Users;
using System.Net.Http.Json;
using System.Net;

namespace Rise.Server.Tests.Controllers.Users
{
    public class UserControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "User")
    {
        [Theory]
        [InlineData("guests", UserRole.Guest, "GET")]
        [InlineData("guests", UserRole.Member, "GET")]
        [InlineData("role/member", UserRole.Guest, "PATCH")]
        [InlineData("role/member", UserRole.Member, "PATCH")]
        public async Task CallUserController_Endpoints_ExpectForbidden(string url, UserRole testLoginRole, string httpMethod)
        {
            await TestForbiddenAccessForEndpoint(url, testLoginRole, httpMethod);
        }

        [Theory]
        [InlineData("guests", "GET")]
        [InlineData("1", "GET")]
        [InlineData("role/member", "PATCH")]
        public async Task Call_UserController_Endpoints_ExpectUnauthorized(string url, string httpMethod)
        {
            await TestUnauthorizedAccessForEndpoint(url, httpMethod);
        }


        [Fact]
        public async Task GET_GuestUsers_ReturnsGuestList()
        {
            await LoginAsync(UserRole.Administrator);

            var response = await _client.GetFromJsonAsync<List<UserDto>>("guests");

            response.ShouldNotBeNull();
            response.ShouldNotBeEmpty();

        }

        [Fact]
        public async Task GET_GuestUsers_ContainsExpectedFields()
        {
            await LoginAsync(UserRole.Administrator);

            var response = await _client.GetFromJsonAsync<List<UserDto>>("guests");


            response.ShouldNotBeNull();
            var firstUser = response.First();

            firstUser.Id.ShouldNotBe(default);
            firstUser.FamilyName.ShouldNotBeNull();

        }

        [Fact]
        public async Task GET_GuestUsers_ReturnsSuccessStatusCode()
        {
            await LoginAsync(UserRole.Administrator);

            var response = await _client.GetAsync("guests");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Fact]
        public async Task GET_GuestUsers_ValidatesResponseFormat()
        {
            await LoginAsync(UserRole.Administrator);

            var response = await _client.GetFromJsonAsync<List<UserDto>>("guests");


            response.ShouldNotBeNull();
            foreach (var user in response)
            {
                user.FamilyName.ShouldNotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async Task GET_UserDetails_ReturnsUserDetails()
        {
            await LoginAsync(UserRole.Guest);

            var response = await _client.GetFromJsonAsync<UserDetailDto>("1");

            response.ShouldNotBeNull();
            response.Id.ShouldBe(1);
            response.FamilyName.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99999)]
        public async Task GET_UserDetails_FailsForInvalidUserId(int userId)
        {
            await LoginAsync(UserRole.Guest);

            var response = await _client.GetAsync(userId.ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [ClassData(typeof(RegisterUserDtoValidData))]
        public async Task POST_RegisterUser_ReturnsUserId(RegisterUserDto userDto)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var response = await _client.PostAsJsonAsync("register", userDto);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var userId = await response.Content.ReadFromJsonAsync<int>();
            userId.ShouldBeGreaterThan(0);

            await DeleteAuth0UserByBuutUserId(userId);
        }

        [Theory]
        [ClassData(typeof(RegisterUserDtoBadData))]
        public async Task POST_RegisterUser_ReturnsBadRequest(RegisterUserDto userDto)
        {
            var response = await _client.PostAsJsonAsync("register", userDto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_RegisterUser_RegisterExistingUser_ReturnConflict()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var userDto = new RegisterUserDto()
            {
                Email = "rand.om@example.com",
                Password = "SecureP@ssw0rd123",
                FirstName = "John",
                FamilyName = "Doe",
                PhoneNumber = "+32471123456",
                Address = new()
                {
                    Street = "Fabiolalaan",
                    Number = "10",
                    City = "Gent",
                    PostalCode = "9000",
                    Country = "Belgium"
                }
            };

            var succesfulResponse = await _client.PostAsJsonAsync("register", userDto);
            var userId = await succesfulResponse.Content.ReadFromJsonAsync<int>();

            var failedResponse = await _client.PostAsJsonAsync("register", userDto);
            failedResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);

            await DeleteAuth0UserByBuutUserId(userId);
        }
    }
}