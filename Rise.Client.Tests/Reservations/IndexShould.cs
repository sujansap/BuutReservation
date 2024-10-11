using MudBlazor.Services;
using Shouldly;
using Xunit.Abstractions;

namespace Rise.Client.Reservations;

/// <summary>
/// These tests are written entirely in C#.
/// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
/// </summary>
public class IndexShould : TestContext
{
    public IndexShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
        Services.AddMudServices();
        Services.AddMudPopoverService();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("_content/Heron.MudCalendar/Heron.MudCalendar.min.js");
    }

    [Fact]
    public void ShowTabs()
    {
        // Arrange
        var cut = RenderComponent<Index>();

        // Assert
        var tabsElement = cut.Find("div.mud-tabs");
        Assert.NotNull(tabsElement);

        cut.FindAll("div.mud-tooltip-root.mud-tooltip-inline").Count.ShouldBe(2);
    }
}

