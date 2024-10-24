using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright.MSTest;
using Xunit;

namespace Rise.Client
{

    [TestClass]
    public class AppTest : PageTest
    {
        private const int DefaultHeight = 1920;

        [TestMethod]
        [DataRow(959)]
        [DataRow(960)]
        public async Task NotFoundPage(int width)
        {
            await Page.SetViewportSizeAsync(width, DefaultHeight);
            await Page.GotoAsync("https://localhost:5001/huh");
            await Expect(Page).ToHaveTitleAsync("Not found");
            await Expect(Page.GetByTestId("page-not-found")).ToHaveTextAsync("Sorry, there's nothing at this address.");
        }
    }
}