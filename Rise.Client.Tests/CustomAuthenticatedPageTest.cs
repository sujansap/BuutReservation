using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

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
            Configuration = new ConfigurationBuilder().AddUserSecrets<CustomAuthenticatedPageTest>().Build();
            GlobalSetup();
        }

        public enum UserRole
        {
            Admin,
            Guest,
            Test
        }

        protected async Task LoginAsync(UserRole role)
        {

            var credentials = role switch
            {
                UserRole.Admin => Configuration.GetSection("Admin").Get<Credentials>(),
                UserRole.Guest => Configuration.GetSection("Guest").Get<Credentials>(),
                UserRole.Test => Configuration.GetSection("Test").Get<Credentials>(),
                _ => throw new ArgumentOutOfRangeException(role.ToString(), "Unknown role")
            };

            await Page.GotoAsync("authentication/login");

            if (credentials == null)
            {
                throw new InvalidOperationException("Credentials cannot be null");
            }

            await Page.FillAsync("input[name='username']", credentials.Email);
            await Page.FillAsync("input[name='password']", credentials.WW);
            await Page.ClickAsync("button[type='submit']");
            await Page.WaitForURLAsync("authentication/callback");

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
            public required string WW { get; set; }
        }
    }
}
