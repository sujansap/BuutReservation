using Rise.Domain.Boats;
using Rise.Domain.Reservations;
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
        battery.Reservations.ShouldBeEmpty();
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

    [Fact]
    public void Initialize_ReservationsCollection_WhenCreated()
    {
        Battery battery = new BatteryBuilder().Build();

        battery.Reservations.ShouldBeEmpty();

        Reservation reservation = new ReservationBuilder().Build();
        battery.Reservations.Add(reservation);

        battery.Reservations.Count.ShouldBe(1);
        battery.Reservations.ShouldContain(reservation);
    }
}




