using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admin
{

    [TestFixture]
    public class AdminGuestsPageTestAdmin : CustomAuthenticatedPageTest
    {
        protected const string baseSuffix = "/admin/guests";

        [SetUp]
        public async Task SetUpAsync()
        {
            await LoginAsync(UserRole.Administrator);
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await LogoutAsync();
            await base.TearDown();
        }

        private async Task MockUsers(UserDto[] users)
        {
            await Page.RouteAsync("*/**/api/User/guests", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(users)
                });
            });
        }

        private async Task InitializeWithMockUsers()
        {
            UserDto[] users =
            [
                new()
                {
                    Id = 1,
                    FamilyName = "Smith"
                },
                new()
                {
                    Id = 2,
                    FamilyName = "Johnson"
                }
            ];

            await MockUsers(users);
            await NavigateToUrl(baseSuffix);
        }

        [Test]
        public async Task HasUserList()
        {
            await NavigateToUrl(baseSuffix);
            await Expect(Page.GetByTestId("user-list")).ToBeVisibleAsync();
        }

        [Test]
        public async Task DisplaysUsersTable()
        {
            await InitializeWithMockUsers();
            await Expect(Page.GetByTestId("users-table")).ToBeVisibleAsync();
        }

        [Test]
        public async Task DisplaysNoUsersMessageWhenEmpty()
        {
            await MockUsers([]);
            await NavigateToUrl(baseSuffix);
            await Expect(Page.GetByTestId("users-none")).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShowsLoadingStateWhileFetchingUsers()
        {
            await Page.RouteAsync("*/**/api/User/guests", async route =>
            {
                await Task.Delay(1000); //dealay  to simulate loading
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(Array.Empty<UserDto>())
                });
            });

            await NavigateToUrl(baseSuffix);
            await Expect(Page.GetByTestId("user-list-loading-progress")).ToBeVisibleAsync();
        }

        [Test]
        public async Task DisplaysErrorMessageOnFailedFetch()
        {
            await Page.RouteAsync("*/**/api/User/guests", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 500,
                    ContentType = "text/plain",
                    Body = "Internal Server Error"
                });
            });

            await NavigateToUrl(baseSuffix);
            await Expect(Page.GetByTestId("user-list-fetch-error")).ToBeVisibleAsync();
        }

        [Test]
        public async Task TableHasCorrectHeaders()
        {
            await InitializeWithMockUsers();

            ILocator headerCell = Page.GetByTestId("users-list-familyName");
            await Expect(headerCell).ToContainTextAsync("Familienaam");
        }

        [Test]
        public async Task DisplaysUserFamilyNames()
        {
            await InitializeWithMockUsers();

            // check if both family names are visible
            //list-guest-page-familyname

            await AssertUserDetails("list-guest-page-familyname-1", "Smith");
            await AssertUserDetails("list-guest-page-familyname-2", "Johnson");

        }


        [Test]
        public async Task RefreshesDataOnReload()
        {
            // initial state
            UserDto[] initialUsers =
            [
                new() { Id = 1, FamilyName = "Smith" }
            ];

            await MockUsers(initialUsers);
            await NavigateToUrl(baseSuffix);
            await Expect(Page.GetByText("Smith")).ToBeVisibleAsync();

            // updated state
            UserDto[] updatedUsers =
            [
                new() { Id = 2, FamilyName = "Johnson" }
            ];

            await MockUsers(updatedUsers);
            await ReloadPage();
            await Expect(Page.GetByText("Johnson")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Smith")).Not.ToBeVisibleAsync();
        }


        private async Task AssertUserDetails(string testId, string expectedValue)
        {
            var locator = Page.GetByTestId(testId);
            await Expect(locator).ToBeVisibleAsync();
            await Expect(locator).ToHaveTextAsync(expectedValue);
        }
    }
}