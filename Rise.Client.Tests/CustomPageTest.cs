using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Rise.Client.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class CustomPageTest : PageTest
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            SetDefaultExpectTimeout(10_000);
        }

        [SetUp]
        public async Task Setup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            bool failed = TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Error
                || TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Failure;

            await Context.Tracing.StopAsync(new()
            {
                Path = failed ? Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "playwright-traces",
                    $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                ) : null,
            });
        }
        public override BrowserNewContextOptions ContextOptions()
        {
            string baseUrl = TestContext.Parameters.Get("BASE_URL", "https://localhost:5003");
            return new()
            {
                Locale = "en-US",
                ColorScheme = ColorScheme.Light,
                BaseURL = baseUrl,
                IgnoreHTTPSErrors = true
            };
        }

        protected async Task InitNavigationToUrl(string url)
        {
            await Page.GotoAsync(url);

            await Page.WaitForSelectorAsync("[data-testid=app-loader]", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Hidden, Timeout = 0 });
        }

    }
}