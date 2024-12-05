using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.Users;
using static Rise.Shared.Users.UserRegistrationModelDto;

namespace Rise.Client.Tests.Admin
{
    [TestFixture]
    public class AdminGuestDetailsPageTestAdmin : CustomAuthenticatedPageTest
    {
        protected const string baseSuffix = "/admin/guests";

        private readonly static string[] fieldNames = ["name", "email", "address", "phone"];

        [SetUp]
        public async Task SetUpAsync()
        {
            base.GlobalSetUp();
            await LoginAsync(UserRole.Administrator);
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await LogoutAsync();
            await base.TearDown();
        }

        private async Task AssertUserDetail(string testId, string expectedValue)
        {
            var locator = Page.GetByTestId(testId);
            await Expect(locator).ToBeVisibleAsync();
            await Expect(locator).ToHaveTextAsync(expectedValue);
        }

        private async Task MockUserDetails(int userId, UserDetailDto userDetails, int? delayMs = null)
        {
            await Page.RouteAsync($"**/api/User/{userId}", async route =>
            {
                if (delayMs.HasValue)
                {
                    await Task.Delay(delayMs.Value);
                }

                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(userDetails)
                });
            });
        }

        private async Task InitializeWithMockUser(int userId = 1, int? delayMs = null)
        {
            var userDetails = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Smith",
                FirstName = "John",
                Email = "",
                PhoneNumber = "",
                Address = new AddressModel
                {
                    Street = "Main",
                    Number = "123",
                    City = "New York",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            await MockUserDetails(userId, userDetails, delayMs);
            await NavigateToUrl($"{baseSuffix}/{userId}");
        }

        [Test]
        public async Task DisplaysUserFamilyName()
        {
            await InitializeWithMockUser(1);
            await AssertUserDetail("user-details-page-familyname", "John Smith");

        }

        [Test]
        public async Task ShowsLoadingStateWhileFetchingDetails()
        {
            await InitializeWithMockUser(1, 2000);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            // loader is too fast so we need to wait ^^ for everything to be loaded
            await Expect(Page.GetByTestId("user-details-loading-progress")).ToBeVisibleAsync();
        }

        [Test]
        public async Task DisplaysErrorMessageOnFailedFetch()
        {
            const int userId = 1;
            await Page.RouteAsync($"**/api/User/{userId}", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 500,
                    ContentType = "text/plain",
                    Body = "Internal Server Error"
                });
            });

            await NavigateToUrl($"{baseSuffix}/{userId}");
            await Expect(Page.GetByTestId("user-details-fetch-error")).ToBeVisibleAsync();
        }

        [Test]
        public async Task NavigatesBackToListOnButtonClick()
        {
            await InitializeWithMockUser(1);

            await Page.GetByTestId("back-to-guests-list-button").ClickAsync();
            await Expect(Page).ToHaveURLAsync(baseSuffix);
        }

        [Test]
        public async Task RefreshesDataOnReload()
        {
            const int userId = 1;
            // initialUser state
            var initialUser = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Smith",
                FirstName = "John",
                Email = "",
                PhoneNumber = "",
                Address = new AddressModel
                {
                    Street = "Main",
                    Number = "123",
                    City = "New York",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            await MockUserDetails(userId, initialUser);
            await NavigateToUrl($"{baseSuffix}/{userId}");
            await AssertUserDetail("user-details-page-familyname", "John Smith");

            // updatedUser state
            var updatedUser = new UserDetailDto
            {
                Id = userId,
                FirstName = "John",
                FamilyName = "Johnson",
                Email = "",
                PhoneNumber = "",
                Address = new AddressModel
                {
                    Street = "Main",
                    Number = "123",
                    City = "New York",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            await MockUserDetails(userId, updatedUser);
            await ReloadPage();
            await AssertUserDetail("user-details-page-familyname", "John Johnson");
        }

        [Test]
        public async Task HandlesInvalidUserIdGracefully()
        {
            const int invalidUserId = 999;
            await Page.RouteAsync($"**/api/User/{invalidUserId}", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 404,
                    ContentType = "text/plain",
                    Body = "User not found"
                });
            });

            await NavigateToUrl($"{baseSuffix}/{invalidUserId}");
            await Expect(Page.GetByTestId("user-details-fetch-error")).ToBeVisibleAsync();
        }


        [Test]
        public async Task DisplaysAllUserFields()
        {
            await InitializeWithMockUser(1);
            await Task.WhenAll(fieldNames
                .Select(field => Expect(Page.GetByTestId($"user-details-page-{field}")).ToBeVisibleAsync()));
        }
    }
}