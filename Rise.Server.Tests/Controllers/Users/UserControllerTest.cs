using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.Users;
using System.Net.Http.Json;
using System.Net;
using Auth0.ManagementApi.Models;
using static Rise.Shared.Users.UserRegistrationModelDto;

namespace Rise.Server.Tests.Controllers.Users
{
    public class UserControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "User")
    {
        [Theory]
        [InlineData("users?role=Guest", UserRole.Guest)]
        [InlineData("users?role=Member", UserRole.Guest)]
        [InlineData("users?role=Administrator", UserRole.Member)]
        [InlineData("users?role=Guest", UserRole.Member)]
        public async Task GetUsersByRole_WithNonAdminRole_ReturnsForbidden(string url, UserRole testLoginRole)
        {
            await LoginAsync(testLoginRole);
            var response = await _client.GetAsync(url);
            response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        [Theory]
        [InlineData("users?role=Guest")]
        [InlineData("users?role=Member")]
        [InlineData("1")]
        public async Task UserEndpoints_WithoutAuthentication_ReturnsUnauthorized(string url)
        {
            var response = await _client.GetAsync(url);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(UserRole.Guest)]
        public async Task GetUsersByRole_AsAdmin_ReturnsCorrectUsers(UserRole roleToQuery)
        {
            await LoginAsync(UserRole.Administrator);

            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine($"Request URL: users?role={roleToQuery}&page=1&pageSize=10");


            var response = await _client.GetFromJsonAsync<UsersPagination<UserDto>>($"?role={roleToQuery}&page=1&pageSize=10");

            response.ShouldNotBeNull();
            response.Items.ShouldNotBeNull();
            response.Items.Count().ShouldBeGreaterThan(0);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 5)]
        [InlineData(1, -5)]
        public async Task GetUsersByRole_WithInvalidPagination_ReturnsBadRequest(int page, int pageSize)
        {
            await LoginAsync(UserRole.Administrator);


            var response = await _client.GetAsync($"users?role={UserRole.Guest}&page={page}&pageSize={pageSize}");

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetUserDetails_WithValidId_ReturnsCorrectUser()
        {
            const int userId = 1;
            await LoginAsync(UserRole.Administrator);


            var response = await _client.GetFromJsonAsync<UserDetailDto>($"{userId}");

            response.ShouldNotBeNull();
            response.Id.ShouldBe(userId);
            response.Email.ShouldNotBeNullOrEmpty();
            response.FirstName.ShouldNotBeNullOrEmpty();
            response.FamilyName.ShouldNotBeNullOrEmpty();
            response.PhoneNumber.ShouldNotBeNullOrEmpty();
            response.Address.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99999)]
        public async Task GetUserDetails_WithInvalidId_ReturnsNotFound(int userId)
        {
            await LoginAsync(UserRole.Administrator);

            var response = await _client.GetAsync($"{userId}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddMemberRole_AsAdmin_SuccessfullyAddsRole()
        {
            await LoginAsync(UserRole.Administrator);

            //create a valid guest user to add member role to
            await RegisterValidAuth0User();

            //validuser has id 3
            var request = new AddMemberRoleDto { UserId = 3, Role = UserRole.Member };

            await Task.Delay(TimeSpan.FromSeconds(2));

            var response = await _client.PostAsJsonAsync("role", request);



            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            //delete the user we created in auth0
            await DeleteAuth0UserByBuutUserId(3);

        }

        [Theory]
        [InlineData(UserRole.Administrator)]
        [InlineData(UserRole.Guest)]
        public async Task AddMemberRole_WithNonMemberRole_ReturnsBadRequest(UserRole roleToAdd)
        {
            // Arrange
            await LoginAsync(UserRole.Administrator);
            var request = new AddMemberRoleDto { UserId = 1, Role = roleToAdd };

            // Act
            var response = await _client.PostAsJsonAsync("role", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [ClassData(typeof(RegisterUserDtoValidData))]
        public async Task POST_RegisterUser_ReturnsUserId(UserRegistrationModelDto userDto)
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
        public async Task POST_RegisterUser_ReturnsBadRequest(UserRegistrationModelDto userDto)
        {
            var response = await _client.PostAsJsonAsync("register", userDto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_RegisterUser_RegisterExistingUser_ReturnConflict()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var userDto = new UserRegistrationModelDto()
            {
                Email = "rand.om@example.com",
                Password = "SecureP@ssw0rd123",
                FirstName = "John",
                FamilyName = "Doe",
                PhoneNumber = "+32471123456",
                DateOfBirth = new DateTime(2005, 1, 1),
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