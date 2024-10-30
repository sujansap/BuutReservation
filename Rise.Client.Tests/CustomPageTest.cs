using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Rise.Client
{
    [TestFixture]
    public class CustomPageTest : PageTest
    {
        public override BrowserNewContextOptions ContextOptions()
        {
            string baseUrl = TestContext.Parameters.Get("BASE_URL", "https://localhost:5003");
            return new()
            {
                Locale = "en-US",
                ColorScheme = ColorScheme.Light,
                BaseURL = baseUrl
            };
        }
    }
}