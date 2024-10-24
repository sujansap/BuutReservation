using Shouldly;
using Xunit;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSubstitute;
using Rise.Domain.Users;
using Rise.Domain.Timeslots;

namespace Rise.Domain.Tests.Timeslots
{
    public class ReservationShould
    {
        // FIXME: DO WE NEED THIS TEST? A RESERVATION CAN'T EXIT WITHOUT THE USER WHO MADE IT WITH THE CURRENT IMPLEMENTATION
        // [Fact]
        // public void BeCreatedWithoutUser()
        // {
        //     // Mock boat
        //     IBoat mockBoat = Substitute.For<IBoat>();

        //     // Mock Time slot
        //     ITimeSlot mockTimeSlot = Substitute.For<ITimeSlot>();

        //     Reservation reservation = new()
        //     {
        //         Boat = mockBoat,
        //         TimeSlot = mockTimeSlot,
        //     };

        //     // Act & Assert
        //     reservation.Boat.ShouldBe(mockBoat);
        //     reservation.TimeSlot.ShouldBe(mockTimeSlot);
        //     reservation.User.ShouldBeEmpty();
        // }

        [Fact]
        public void BeCreatedWithUser()
        {
            // Mock boat
            IBoat mockBoat = Substitute.For<IBoat>();

            // Mock Time slot
            ITimeSlot mockTimeSlot = Substitute.For<ITimeSlot>();

            IUser mockUser = Substitute.For<IUser>();

            Reservation reservation = new()
            {
                User = mockUser,
                Boat = mockBoat,
                TimeSlot = mockTimeSlot,
            };

            // Act & Assert
            reservation.Boat.ShouldBe(mockBoat);
            reservation.TimeSlot.ShouldBe(mockTimeSlot);
            reservation.User.ShouldBe(mockUser);
        }
    }

}