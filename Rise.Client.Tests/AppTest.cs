
namespace Rise.Client.Tests
{

    [TestFixture]
    public class AppTest : CustomPageTest
    {
        private const int DefaultHeight = 1920;

        [Test]
        [TestCase(959)]
        [TestCase(960)]
        public async Task NotFoundPage(int width)
        {
            await Page.SetViewportSizeAsync(width, DefaultHeight);
            await Page.GotoAsync("/huh");
            await Expect(Page).ToHaveTitleAsync("Not found", new() { Timeout = 15000 });
            await Expect(Page.GetByTestId("page-not-found")).ToHaveTextAsync("Sorry, there's nothing at this address.");
        }
    }
}