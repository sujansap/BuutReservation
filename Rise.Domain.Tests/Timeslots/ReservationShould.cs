using Moq;
using Shouldly;
using Xunit;
using Rise.Domain.Boats;
using Rise.Domain.Timeslots;


namespace Rise.Domain.Tests.Timeslots
{
    public class ReservationShould
    {
        private static readonly int validAmountChildren = 1;
        private static readonly int validAmountPets = 1;

        [Fact]
        public void BeCreated()
        {
            // Arrange
            var mockBoat = new Mock<IBoat>();
            mockBoat.SetupGet(b => b.PersonalName).Returns("Boat");
            // mockBoat.SetupGet(b => b.MaximumAdults).Returns(2);
            // mockBoat.SetupGet(b => b.MaximumChildren).Returns(1);
            // mockBoat.SetupGet(b => b.MaximumPets).Returns(1);

            var boat = mockBoat.Object;

            Reservation reservation = new()
            {
                Boat = boat,
                // BatteryId = 1,
                // TimeSlotId = 1,
                AmountAdults = 2,
                AmountChildren = validAmountChildren,
                AmountPets = validAmountPets
            };

            // Act & Assert
            reservation.Boat.PersonalName.ShouldBe("Boat");
            // reservation.Boat.MaximumAdults.ShouldBe(2);
            // reservation.Boat.MaximumChildren.ShouldBe(1);
            // reservation.Boat.MaximumPets.ShouldBe(1);
            // reservation.BatteryId.ShouldBe(1);
            // reservation.TimeSlotId.ShouldBe(1);
            reservation.AmountAdults.ShouldBe(2);
            reservation.AmountChildren.ShouldBe(validAmountChildren);
            reservation.AmountPets.ShouldBe(validAmountPets);
        }
    }

}