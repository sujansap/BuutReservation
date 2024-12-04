
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Rise.Domain.Users;
using Rise.Server.Tests.Utils;
using Rise.Shared.Users;
using Shouldly;

namespace Rise.Server.Tests.Fixtures
{
    [Trait("Category", "Integration")]
    public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
    {

        // private readonly Checkpoint _checkpoint = new()
        // {
        //     SchemasToInclude = [
        //         "Boat",
        //         "CruisePeriods",
        //         "Reservation",
        //         "ReservationUser",
        //         "TimeSlots",
        //         "User",
        // ],
        //     DbAdapter = DbAdapter.Postgres,
        //     WithReseed = true
        // };
        protected readonly ApiWebApplicationFactory _factory;
        protected readonly HttpClient _client;
        private readonly AuthenticationApiClient _authenticationApiClient;
        private readonly IManagementApiClient _managementApiClient;
        private readonly List<string> _createdUserIds = [];

        public IntegrationTest(ApiWebApplicationFactory fixture, string routeBase)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri(_client.BaseAddress ?? new Uri("https://localhost"), "api/" + routeBase + "/");
            var config = fixture.Configuration;

            _managementApiClient = fixture.Services.GetRequiredService<IManagementApiClient>();

            var auth0Domain = config["Auth0:Authority"];
            if (string.IsNullOrEmpty(auth0Domain))
            {
                throw new ArgumentException("Auth0 domain is not configured.");
            }

            _authenticationApiClient = new AuthenticationApiClient(new Uri(auth0Domain));
            // TODO set up respawn for avoiding changes during tests https://github.com/jbogard/respawn
        }

        public async Task DisposeAsync()
        {
            foreach (var userId in _createdUserIds)
            {
                try
                {
                    await _managementApiClient.Users.DeleteAsync(userId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete user with ID {userId}: {ex.Message}");
                }
            }

            _createdUserIds.Clear();
        }

        public async Task InitializeAsync()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                await CreateUserWithRole(role);
            }
        }

        protected async Task LoginAsync(UserRole testLoginRole)
        {
            var clientId = _factory.Configuration["Auth0:BlazorClientId"];
            var clientSecret = _factory.Configuration["Auth0:BlazorClientSecret"];
            var audience = _factory.Configuration["Auth0:Audience"];

            var tokenRequest = new ResourceOwnerTokenRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = "openid profile email",
                Audience = audience,
                Username = testLoginRole.GetEmail(),
                Password = testLoginRole.GetPassword(),
            };

            var task = SendCreateUserRequest(testLoginRole);
            await RunTaskWithRetries(async () => await SendLoginRequest(tokenRequest), 50);
        }

        private async Task<bool> SendLoginRequest(ResourceOwnerTokenRequest tokenRequest)
        {
            try
            {
                var tokenResponse = await _authenticationApiClient.GetTokenAsync(tokenRequest);
                var token = tokenResponse.AccessToken;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Rate limit exceeded. Retrying after 1 seconds...");
                //Delay so that auth0 api doesn't throw a rate limit exception
                await Task.Delay(TimeSpan.FromSeconds(1));
                return false;
            }
        }

        private async Task CreateUserWithRole(UserRole testLoginRole)
        {
            try
            {
                var task = SendCreateUserRequest(testLoginRole);
                await RunTaskWithRetries(async () => await SendCreateUserRequest(testLoginRole), 50);
            }
            catch (ErrorApiException)
            {
                // User already exists
                return;
            }
        }

        private async Task<bool> SendCreateUserRequest(UserRole testLoginRole)
        {
            try
            {
                var user = await _managementApiClient.Users.CreateAsync(new UserCreateRequest
                {
                    UserName = testLoginRole.GetUserName(),
                    Email = testLoginRole.GetEmail(),
                    Connection = "Username-Password-Authentication",
                    Password = testLoginRole.GetPassword(),
                    AppMetadata = new Dictionary<string, object>
                        {
                            { "buutUserId", "1" },
                        }
                });

                _createdUserIds.Add(user.UserId);

                var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = testLoginRole.GetRole() });
                var role = roles.FirstOrDefault() ?? throw new Exception($"Role '{testLoginRole.GetRole()}' not found");
                await _managementApiClient.Users.AssignRolesAsync(user.UserId, new AssignRolesRequest
                {
                    Roles = [role.Id]
                });

                return true;
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Rate limit exceeded. Retrying after 2 seconds...");
                //Delay so that auth0 api doesn't throw a rate limit exception
                await Task.Delay(TimeSpan.FromSeconds(2));
                return false;
            }
        }

        private static async Task RunTaskWithRetries(Func<Task<bool>> callback, int retryLimit)
        {
            var retries = 0;
            var success = false;
            while (retries <= retryLimit && !success)
            {
                success = await callback();
                retries++;
            }
        }

        protected async Task TestForbiddenAccessForEndpoint(string url, UserRole testLoginRole, string httpMethod)
        {
            await LoginAsync(testLoginRole);

            var response = await GetResponseForRequest(url, httpMethod);

            response?.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        protected async Task TestUnauthorizedAccessForEndpoint(string url, string httpMethod)
        {
            var response = await GetResponseForRequest(url, httpMethod);

            response?.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        private async Task<HttpResponseMessage?> GetResponseForRequest(string url, string httpMethod) => httpMethod switch
        {
            "GET" => await _client.GetAsync(url),
            "POST" => await _client.PostAsJsonAsync(url, new object()),
            "PTACH" => await _client.PatchAsJsonAsync(url, new object()),
            _ => null,
        };

        private async Task<Auth0.ManagementApi.Models.User?> FindUserByBuutUserId(int buutUserId)
        {
            var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest() { Query = $"app_metadata.buutUserId:{buutUserId}" });
            return users.FirstOrDefault();
        }

        public async Task DeleteAuth0UserByBuutUserId(int buutUserId)
        {
            try
            {
                await RunTaskWithRetries(async () => await SendDeleteAuth0UserRequest(buutUserId), 50);
            }
            catch (Exception ex)
            {
                throw new Exception("Delete request failed.", ex);
            }
        }

        private async Task<bool> SendDeleteAuth0UserRequest(int buutUserId)
        {
            try
            {
                var user = await FindUserByBuutUserId(buutUserId);
                if (user != null)
                {
                    await _managementApiClient.Users.DeleteAsync(user.UserId);
                    return true;
                }
                return false;
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Rate limit exceeded. Retrying after 2 seconds...");
                //Delay so that auth0 api doesn't throw a rate limit exception
                await Task.Delay(TimeSpan.FromSeconds(2));
                return false;
            }
        }

        public async Task RegisterValidAuth0User()
        {
            await RunTaskWithRetries(async () => await RegisterValidUser(), 20);
        }

        private async Task<bool> RegisterValidUser()
        {
            try
            {

                const int buutUserId = 3;
                const string email = "user3@example.com";
                const string password = "SecureP@ssw0rd123!";
                const UserRole roleName = UserRole.Guest;

                // Create the user
                var user = await _managementApiClient.Users.CreateAsync(new UserCreateRequest
                {
                    UserName = email,
                    Email = email,
                    Connection = "Username-Password-Authentication",
                    Password = password,
                    AppMetadata = new Dictionary<string, object>
            {
                { "buutUserId", buutUserId }
            }
                });

                _createdUserIds.Add(user.UserId);

                // Assign a role to the user
                var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = roleName.ToString() });
                var role = roles.FirstOrDefault() ?? throw new Exception($"Role '{roleName}' not found");

                await _managementApiClient.Users.AssignRolesAsync(user.UserId, new AssignRolesRequest
                {
                    Roles = new[] { role.Id }
                });

                Console.WriteLine($"User with buutUserId {buutUserId} created successfully.");

                return true;
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine($"Rate limit exceeded: {ex.Message}. Retrying...");
                await Task.Delay(TimeSpan.FromSeconds(2));
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create user: {ex.Message}");
                return false;

            }

        }



    }
}
