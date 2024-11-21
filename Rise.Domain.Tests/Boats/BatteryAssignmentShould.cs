using Rise.Domain.Tests.TestUtilities;
using Shouldly;

namespace Rise.Domain.Tests.Boats;

public class BatteryAssignmentShould
{
    [Fact]
    public void AssignBatteryToReservation()
    {
        // Arrange
        var boat = new BoatBuilder().Build();
        var battery = new BatteryBuilder().WithBoat(boat).Build();
        var reservation = new ReservationBuilder().WithBoat(boat).Build();

        // Act
        reservation.Battery = battery;

        // Assert
        reservation.Battery.ShouldBe(battery);
        reservation.BatteryId.ShouldBe(battery.Id);
    }

    [Fact]
    public void TrackBatteryUsageCount()
    {
        // Arrange
        var boat = new BoatBuilder().Build();
        var battery = new BatteryBuilder().WithBoat(boat).Build();
        var reservation = new ReservationBuilder().WithBoat(boat).Build();

        // Act
        reservation.Battery = battery;

        // Assert
        battery.Reservations.Count.ShouldBe(1);
        battery.Reservations.ShouldContain(reservation);
    }
} 