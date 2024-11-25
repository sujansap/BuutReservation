using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Rise.Client.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class CustomAuthenticatedPageTest : CustomPageTest
    {
        protected WebAssemblyHostBuilder Builder { get; private set; } = default!;

        [OneTimeSetUp]
        public new void GlobalSetup()
        {
            Builder = WebAssemblyHostBuilder.CreateDefault();
            base.GlobalSetup();
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            var options = base.ContextOptions();
            options.StorageStatePath = "./Playwright/.auth/state.json";
            return options;
        }
    }
}
