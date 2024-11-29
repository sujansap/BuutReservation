using Microsoft.Extensions.Configuration;
using Rise.Shared.Users;

namespace Rise.Client.Tests
{
    [TestFixture]
    public class CustomAuthenticatedPageTest : CustomPageTest
    {
        protected IConfiguration Configuration { get; private set; } = default!;

        private string? SessionStorage
        {
            get;
            set;
        }

        [OneTimeSetUp]
        public override void GlobalSetUp()
        {
            var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<CustomAuthenticatedPageTest>();
            Configuration = builder.Build();


            base.GlobalSetUp();
        }

        protected async Task LoginAsync(UserRole role)
        {
            if (IsLoggedIn())
            {
                await InjectSessionStorage();
                return;
            }

            Credentials? credentials = role switch
            {
                UserRole.Administrator => Configuration.GetSection("Administrator").Get<Credentials>(),
                UserRole.Guest => Configuration.GetSection("Guest").Get<Credentials>(),
                UserRole.Member => Configuration.GetSection("Member").Get<Credentials>(),
                _ => null
            } ?? throw new InvalidOperationException("Credentials cannot be null");

            await LoginUsingCredentials(credentials);
        }

        private async Task LoginUsingCredentials(Credentials credentials)
        {
            await NavigateToUrl("/authentication/login");

            await Page.FillAsync("input[name='username']", credentials.Email);
            await Page.FillAsync("input[name='password']", credentials.Password);
            await Page.ClickAsync("button[type='submit']:not(.ulp-hidden-form-submit-button)");

            await SaveSessionStorage();
            await NavigateToUrl("/home");
        }

        private async Task SaveSessionStorage()
        {
            string sessionStorage = await Page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");
            SessionStorage = sessionStorage;
        }

        private bool IsLoggedIn()
        {
            return SessionStorage?.Contains("oidc.user:https://rise-gent2.eu.auth0.com:8vJtbXg2FptHGmKrpFl1tZwhiXOJZ57l") ?? false;
        }

        private async Task InjectSessionStorage()
        {
            await Context.AddInitScriptAsync(@"(storage => {
                if (window.location.hostname === 'localhost') {
                    const entries = JSON.parse(storage);
                    for (const [key, value] of Object.entries(entries)) {
                        window.sessionStorage.setItem(key, value);
                    }
                }
            })('" + SessionStorage + "')");
        }

        private class Credentials
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        protected async Task LogoutAsync()
        {
            await NavigateToUrl("/authentication/logout");
            SessionStorage = null;
        }

        protected async Task CheckRedirectedToLogin()
        {
            await Expect(Page.GetByText("Log in to Buut")).ToBeVisibleAsync();

        }

        protected async Task TestRedirectWhenNotLoggedIn(string url)
        {
            await NavigateToUrl(url);
            await CheckRedirectedToLogin();
        }
        protected async Task TestNotAuthorized(string url, UserRole role)
        {
            await LoginAsync(role);
            await NavigateToUrl(url);
            await Expect(Page.GetByTestId("unauthorized")).ToBeVisibleAsync();
            await LogoutAsync();
        }
    }
}
