using Rise.Domain.Tests.TestUtilities;
using Shouldly;
using Rise.Domain.Common;
using Rise.Domain.Reservations;
using Rise.Domain.Boats;

namespace Rise.Domain.Tests.Boats;

public class BatteryAssignmentTests
{
    [Fact]
    public async Task AssignBatteriesToReservations_ShouldAssignBatteriesCorrectly()
    {
        // Arrange
        var validBoatId = 1;
        var reservations = new List<Reservation>
        {
            new Reservation 
            { 
                BoatId = validBoatId,
                // ... other properties
            }
        };

        var batteries = new List<Battery>
        {
            new Battery 
            { 
                BoatId = validBoatId,
                // ... other properties
            }
        };

        var timeInfo = new TimeInfo(DateTime.UtcNow);

        // Act
        await Battery.AssignBatteriesToReservationsAsync(reservations, batteries, timeInfo);

        // Assert
        // ... your assertions
    }

    [Fact]
    public void Battery_ShouldNotBeAvailable_WhenRecentlyUsed()
    {
        // Arrange
        var boat = new BoatBuilder().Build();
        var battery = new BatteryBuilder().WithBoat(boat).Build();
        var currentTime = DateTime.UtcNow;
        
        var reservation = new ReservationBuilder()
            .WithBoat(boat)
            .Build();
        battery.AddReservation(reservation);

        var date = DateOnly.FromDateTime(currentTime);
        var startTime = TimeOnly.FromDateTime(currentTime.AddHours(2));
        var endTime = TimeOnly.FromDateTime(currentTime.AddHours(4));

        // Act
        var isAvailable = battery.IsAvailableFor(date, startTime, endTime, currentTime);

        // Assert
        isAvailable.ShouldBeFalse();
    }

    [Fact]
    public void Battery_ShouldBeAvailable_AfterSufficientChargingTime()
    {
        // Arrange
        var boat = new BoatBuilder().Build();
        var battery = new BatteryBuilder().WithBoat(boat).Build();
        var currentTime = DateTime.UtcNow;
        
        var reservation = new ReservationBuilder()
            .WithBoat(boat)
            .Build();
        battery.AddReservation(reservation);

        var futureTime = currentTime.AddHours(5); // More than 4 hours charging time
        
        var date = DateOnly.FromDateTime(futureTime);
        var startTime = TimeOnly.FromDateTime(futureTime.AddHours(1));
        var endTime = TimeOnly.FromDateTime(futureTime.AddHours(3));

        // Act
        var isAvailable = battery.IsAvailableFor(date, startTime, endTime, futureTime);

        // Assert
        isAvailable.ShouldBeTrue();
    }

    [Fact]
    public void HandleCompletedReservations_ShouldResetBatteryAssignments()
    {
        // Arrange
        var boat = new BoatBuilder().Build();
        var battery = new BatteryBuilder().WithBoat(boat).Build();
        var user = new UserBuilder().Build();
        var reservation = new ReservationBuilder()
            .WithBoat(boat)
            .WithUser(user)
            .Build();
        
        reservation.Battery = battery;
        var completedReservations = new List<Reservation> { reservation };

        // Act
        Battery.HandleCompletedReservations(completedReservations);

        // Assert
        reservation.Battery.ShouldBeNull();
        battery.CurrentHolder.ShouldBe(user);
    }

    [Fact]
    public void GetCompatibleBatteries_ShouldReturnCorrectBatteries()
    {
        // Arrange
        var boat1 = new BoatBuilder().Build();
        var boat2 = new BoatBuilder().Build();
        
        var battery1 = new BatteryBuilder().WithBoat(boat1).Build();
        var battery2 = new BatteryBuilder().WithBoat(boat1).Build();
        var battery3 = new BatteryBuilder().WithBoat(boat2).Build();

        var batteries = new List<Battery> { battery1, battery2, battery3 };

        // Act
        var compatibleBatteries = Battery.GetCompatibleBatteriesForBoat(batteries, boat1.Id).ToList();

        // Assert
        compatibleBatteries.Count.ShouldBe(2);
        compatibleBatteries.ShouldContain(battery1);
        compatibleBatteries.ShouldContain(battery2);
        compatibleBatteries.ShouldNotContain(battery3);
    }

    [Fact]
    public void Battery_ShouldBeCreatedWithValidProperties()
    {
        // Arrange & Act
        Battery battery = new BatteryBuilder().Build();

        // Assert
        battery.Type.ShouldBe(BatteryBuilder.ValidBatteryType);
        battery.Boat.ShouldBe(BatteryBuilder.ValidBoat);
        battery.Mentor.ShouldBe(BatteryBuilder.ValidMentor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Battery_ShouldNotBeCreated_WithInvalidType(string? invalidType)
    {
        // Arrange & Act
        Action act = () =>
        {
            Battery battery = new BatteryBuilder()
                .WithBatteryType(invalidType!)
                .Build();
        };

        // Assert
        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Type");
    }
}
