using Shouldly;
using Xunit;
using Rise.Domain.Boats;
using Rise.Domain.Timeslots;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSubstitute;
using Rise.Domain.Users;


namespace Rise.Domain.Tests.Timeslots
{
    public class ReservationShould
    {
        public const int validAmountAdults = 2;
        public const int validAmountChildren = 1;
        public const int validAmountPets = 1;

        [Fact]
        public void BeCreatedWithoutUsers()
        {
            // Mock boat
            IBoat mockBoat = Substitute.For<IBoat>();

            // Mock Time slot
            ITimeSlot mockTimeSlot = Substitute.For<ITimeSlot>();

            Reservation reservation = new()
            {
                AmountAdults = validAmountAdults,
                AmountChildren = validAmountChildren,
                AmountPets = validAmountPets,
                Boat = mockBoat,
                TimeSlot = mockTimeSlot,
            };

            // Act & Assert
            reservation.AmountAdults.ShouldBe(validAmountAdults);
            reservation.AmountChildren.ShouldBe(validAmountChildren);
            reservation.AmountPets.ShouldBe(validAmountPets);

            reservation.Boat.ShouldBe(mockBoat);
            reservation.TimeSlot.ShouldBe(mockTimeSlot);
            reservation.Users.ShouldBeEmpty();
        }

        [Fact]
        public void BeCreatedWithUsers()
        {
            // Mock boat
            IBoat mockBoat = Substitute.For<IBoat>();

            // Mock Time slot
            ITimeSlot mockTimeSlot = Substitute.For<ITimeSlot>();

            IUser mockUser = Substitute.For<IUser>();

            Reservation reservation = new([mockUser])
            {
                AmountAdults = validAmountAdults,
                AmountChildren = validAmountChildren,
                AmountPets = validAmountPets,
                Boat = mockBoat,
                TimeSlot = mockTimeSlot,
            };

            // Act & Assert
            reservation.AmountAdults.ShouldBe(validAmountAdults);
            reservation.AmountChildren.ShouldBe(validAmountChildren);
            reservation.AmountPets.ShouldBe(validAmountPets);

            reservation.Boat.ShouldBe(mockBoat);
            reservation.TimeSlot.ShouldBe(mockTimeSlot);
            reservation.Users.ShouldContain(mockUser);
        }
    }

}