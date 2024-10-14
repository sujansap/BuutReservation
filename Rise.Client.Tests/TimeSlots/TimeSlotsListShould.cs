using Rise.Shared.TimeSlots;
using Xunit.Abstractions;
using Shouldly;
using System.Collections.Generic;
using Rise.Client.Pages;
using Rise.Client.Components;

namespace Rise.Client.TimeSlots;

public class TimeSlotsListShould : TestContext
{
    public TimeSlotsListShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
        Services.AddScoped<ITimeSlotService, FakeTimeSlotService>();
    }

    [Fact]
    public void ShowsTimeSlotsList()
    {
        var cut = RenderComponent<TimeSlotsList>();
        cut.FindAll(".test-time-slot-card").Count.ShouldBe(5);
    }
}