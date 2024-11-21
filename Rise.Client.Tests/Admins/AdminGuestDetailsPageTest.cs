using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.Users;
using Shouldly;

namespace Rise.Client.Tests.Admin
{
    [TestFixture]
    public class AdminGuestDetailsPageTest : CustomPageTest
    {

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
                FamilyName = "Smith"
                /*later more here*/
            };

            await MockUserDetails(userId, userDetails, delayMs);
            await InitNavigationToUrl($"/admin/guests/{userId}");
        }

        [Test]
        public async Task DisplaysUserFamilyName()
        {
            await InitializeWithMockUser(1);
            await AssertUserDetail("user-details-page-familyname", "Smith");

        }

        [Test]
        public async Task ShowsLoadingStateWhileFetchingDetails()
        {
            await InitializeWithMockUser(1, 2000);
            await Page.GetByTestId("user-details-loading-progress").IsVisibleAsync();
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

            await InitNavigationToUrl($"/admin/guests/{userId}");
            await Page.GetByTestId("user-details-fetch-error").IsVisibleAsync();
        }

        [Test]
        public async Task NavigatesBackToListOnButtonClick()
        {
            await InitializeWithMockUser(1);

            await Page.GetByTestId("back-to-guests-list-button").ClickAsync();
            await Expect(Page).ToHaveURLAsync("/admin/guests");
        }

        [Test]
        public async Task RefreshesDataOnReload()
        {
            const int userId = 1;
            // initialUser state
            var initialUser = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Smith"
                /*later more here*/
            };

            await MockUserDetails(userId, initialUser);
            await InitNavigationToUrl($"/admin/guests/{userId}");
            await AssertUserDetail("user-details-page-familyname", "Smith");

            // updatedUser state
            var updatedUser = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Johnson"
                /*later more here*/
            };

            await MockUserDetails(userId, updatedUser);
            await Page.ReloadAsync();
            await AssertUserDetail("user-details-page-familyname", "Johnson");
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

            await InitNavigationToUrl($"/admin/guests/{invalidUserId}");
            await Page.GetByTestId("user-details-fetch-error").IsVisibleAsync();
        }
    }
}