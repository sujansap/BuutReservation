using Rise.Shared.Users;

namespace Rise.Client.Tests.Admin
{
    [TestFixture]
    public class AdminGuestsPageTestAll : CustomAuthenticatedPageTest
    {

        [Test]
        public async Task RedirectWhenNotLoggedIn()
        {
            await NavigateToUrl("/admin/guests");

            await Expect(Page.GetByTestId("login-in-progress")).ToBeVisibleAsync();
        }

        [Test]
        [TestCase(UserRole.Guest)]
        [TestCase(UserRole.Member)]
        public async Task NotAuthorized(UserRole role)
        {
            await LoginAsync(role);
            await NavigateToUrl("/admin/guests");
            await Expect(Page.GetByTestId("unauthorized")).ToBeVisibleAsync();
            await LogoutAsync();
        }
    }
}