using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.Users;
using Shouldly;

namespace Rise.Client.Tests.Admin
{
    [TestFixture]
    public class AdminGuestsPageTest : CustomPageTest
    {
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
            await InitNavigationToUrl("/admin/guests");
        }

        [Test]
        public async Task HasUserList()
        {
            await InitNavigationToUrl("/admin/guests");
            await Page.GetByTestId("user-list").IsVisibleAsync();
        }

        [Test]
        public async Task DisplaysUsersTable()
        {
            await InitializeWithMockUsers();
            await Page.GetByTestId("users-table").IsVisibleAsync();
        }

        [Test]
        public async Task DisplaysNoUsersMessageWhenEmpty()
        {
            await MockUsers(Array.Empty<UserDto>());
            await InitNavigationToUrl("/admin/guests");
            await Page.GetByTestId("users-none").IsVisibleAsync();
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

            await InitNavigationToUrl("/admin/guests");
            await Page.GetByTestId("user-list-loading-progress").IsVisibleAsync();
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

            await InitNavigationToUrl("/admin/guests");
            await Page.GetByTestId("user-list-fetch-error").IsVisibleAsync();
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
            await Expect(Page.GetByText("Smith")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Johnson")).ToBeVisibleAsync();
        }


        [Test]
        public async Task TableIsStripedAndDense()
        {
            await InitializeWithMockUsers();

            ILocator table = Page.GetByTestId("users-table");
            string className = await table.GetAttributeAsync("class");


            className.ShouldContain("mud-table-dense");
            className.ShouldContain("mud-table-striped");
            className.ShouldContain("mud-table-hover");
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
            await InitNavigationToUrl("/admin/guests");
            await Expect(Page.GetByText("Smith")).ToBeVisibleAsync();

            // updated state
            UserDto[] updatedUsers =
            [
                new() { Id = 2, FamilyName = "Johnson" }
            ];

            await MockUsers(updatedUsers);
            await Page.ReloadAsync();
            await Expect(Page.GetByText("Johnson")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Smith")).Not.ToBeVisibleAsync();
        }
    }
}