using System.Text.Json;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admin
{
    [TestFixture]
    public class AdminGuestDetailsPageTest : CustomPageTest
    {
        private async Task MockUserDetails(int userId, UserDetailDto userDetails)
        {
            await Page.RouteAsync($"**/api/User/{userId}", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(userDetails)
                });
            });
        }

        private async Task InitializeWithMockUser(int userId = 1)
        {
            var userDetails = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Smith"
                /*later more here*/
            };

            await MockUserDetails(userId, userDetails);
            await InitNavigationToUrl($"/admin/guests/{userId}");
        }

        [Test]
        public async Task DisplaysUserFamilyName()
        {
            await InitializeWithMockUser(1);
            await Page.GetByTestId("user-details-page-familyname").IsVisibleAsync();
            await Expect(Page.GetByText("Smith")).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShowsLoadingStateWhileFetchingDetails()
        {
            const int userId = 1;
            await Page.RouteAsync($"**/api/User/{userId}", async route =>
            {
                await Task.Delay(1000);
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(
                        new UserDetailDto
                        {
                            Id = userId,
                            FamilyName = "Smith"
                            /*later more here*/
                        }
                    )
                });
            });

            await InitNavigationToUrl($"/admin/guests/{userId}");
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
                /*later more here...*/
            };

            await MockUserDetails(userId, initialUser);
            await InitNavigationToUrl($"/admin/guests/{userId}");
            await Expect(Page.GetByText("Smith")).ToBeVisibleAsync();

            // updatedUser state
            var updatedUser = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Johnson"
            };

            await MockUserDetails(userId, updatedUser);
            await Page.ReloadAsync();
            await Expect(Page.GetByText("Johnson")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Smith")).Not.ToBeVisibleAsync();
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