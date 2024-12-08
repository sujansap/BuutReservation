using Rise.Domain.Boats;
using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Boats;

public class BatteryShould
{
    [Fact]
    public void BeCreated()
    {
        Battery battery = new BatteryBuilder().Build();

        battery.Type.ShouldBe(BatteryBuilder.ValidBatteryType);
        battery.Boat.ShouldBe(BatteryBuilder.ValidBoat);
        battery.Mentor.ShouldBe(BatteryBuilder.ValidMentor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreated_WithInvalidType(string? invalidType)
    {

        Action act = () =>
        {
            Battery battery = new BatteryBuilder()
            .WithBatteryType(invalidType!)
            .Build();
        };


        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Type");
    }

    [Theory]
    [InlineData(null)]
    public void NotBeCreated_WithInvalidMentor(User? mentor)
    {

        Action act = () =>
        {
            Battery battery = new BatteryBuilder()
            .WithMentor(mentor!)
            .Build();
        };


        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Mentor");
    }
}




