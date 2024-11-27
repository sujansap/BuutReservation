using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Rise.Shared.Users;

namespace Rise.Client.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class CustomAuthenticatedPageTest : CustomPageTest
    {
        protected IConfiguration Configuration { get; private set; } = default!;

        [OneTimeSetUp]
        public new void GlobalSetup()
        {
            var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<CustomAuthenticatedPageTest>();
            Configuration = builder.Build();
            base.GlobalSetup();
        }

        protected async Task LoginAsync(UserRole role)
        {

            var credentials = role switch
            {
                UserRole.Administrator => Configuration.GetSection("Administrator").Get<Credentials>(),
                UserRole.Guest => Configuration.GetSection("Guest").Get<Credentials>(),
                UserRole.Member => Configuration.GetSection("Member").Get<Credentials>(),
                _ => throw new ArgumentOutOfRangeException(role.ToString(), "Unknown role")
            };

            await InitNavigationToUrl("/authentication/login");

            if (credentials == null)
            {
                throw new InvalidOperationException("Credentials cannot be null");
            }

            await Page.FillAsync("input[name='username']", credentials.Email);
            await Page.FillAsync("input[name='password']", credentials.Password);
            await Page.ClickAsync("button[type='submit']:not(.ulp-hidden-form-submit-button)");
            await InitNavigationToUrl("/home");

            var sessionStorage = await Page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");
            Environment.SetEnvironmentVariable("SESSION_STORAGE", sessionStorage);

            var loadedSessionStorage = Environment.GetEnvironmentVariable("SESSION_STORAGE");
            await Context.AddInitScriptAsync(@"(storage => {
                if (window.location.hostname === 'localhost') {
                    const entries = JSON.parse(storage);
                    for (const [key, value] of Object.entries(entries)) {
                        window.sessionStorage.setItem(key, value);
                    }
                }
            })('" + loadedSessionStorage + "')");
        }

        private class Credentials
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        protected async Task LogoutAsync()
        {
            await InitNavigationToUrl("/authentication/logout");
            Environment.SetEnvironmentVariable("SESSION_STORAGE", string.Empty);
        }
    }
}
