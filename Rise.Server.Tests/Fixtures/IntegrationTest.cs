
using System.Net.Http.Headers;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rise.Server.Tests.Utils;

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
            foreach (TestLoginRole role in Enum.GetValues(typeof(TestLoginRole)))
            {
                await CreateUserWithRole(role);
            }
        }

        protected async Task LoginAsync(TestLoginRole testLoginRole)
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

            var retries = 0;
            var retryLimit = 50;
            var success = false;
            while (retries <= retryLimit && !success)
            {
                success = await SendLoginRequest(tokenRequest);
                retries++;
            }
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

        private async Task CreateUserWithRole(TestLoginRole testLoginRole)
        {
            try
            {
                var retries = 0;
                var retryLimit = 50;
                var success = false;
                while (retries <= retryLimit && !success)
                {
                    success = await SendCreateUserRequest(testLoginRole);
                    retries++;
                }
            }
            catch (ErrorApiException)
            {
                // User already exists
                return;
            }
            catch
            {
                //Unexpected error
                return;
            }

        }

        private async Task<bool> SendCreateUserRequest(TestLoginRole testLoginRole)
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
                Console.WriteLine($"Rate limit exceeded. Retrying after 1 seconds...");
                //Delay so that auth0 api doesn't throw a rate limit exception
                await Task.Delay(TimeSpan.FromSeconds(1));
                return false;
            }
        }

        protected void Logout()
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
