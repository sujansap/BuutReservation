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

        [Fact]
        public void CanCreateReservationWithValidData()
        {
            IBoat mockBoat = Substitute.For<IBoat>();
            ITimeSlot mockTimeSlot = Substitute.For<ITimeSlot>();
            IUser mockUser = Substitute.For<IUser>();

            var reservation = new Reservation
            {
                Boat = mockBoat,
                BoatId = 1,
                TimeSlot = mockTimeSlot,
                TimeSlotId = 2,
                User = mockUser,
                UserId = 3
            };

            reservation.Boat.ShouldNotBeNull();
            reservation.TimeSlot.ShouldNotBeNull();
            reservation.User.ShouldNotBeNull();
        }

        [Fact]
        public void CannotCreateReservationIfBoatIsAlreadyReservedForTimeSlot()
        {
            IBoat mockBoat = Substitute.For<IBoat>();
            ITimeSlot mockTimeSlot = Substitute.For<ITimeSlot>();
            IUser mockUser = Substitute.For<IUser>();

            var reservation1 = new Reservation
            {
                Boat = mockBoat,
                BoatId = 1,
                TimeSlot = mockTimeSlot,
                TimeSlotId = 2,
                User = mockUser,
                UserId = 3
            };

            var reservation2 = new Reservation
            {
                Boat = mockBoat,
                BoatId = 1,
                TimeSlot = mockTimeSlot,
                TimeSlotId = 2,
                User = mockUser,
                UserId = 4
            };

            CheckBoatAvailability(reservation1, reservation2).ShouldBeFalse("Boat should not be available for the same time slot.");
        }

        [Fact]
        public void ShouldEnforceMinimumDaysBetweenReservations()
        {
            IBoat mockBoat = Substitute.For<IBoat>();
            IUser mockUser = Substitute.For<IUser>();

            ITimeSlot mockTimeSlot1 = Substitute.For<ITimeSlot>();
            mockTimeSlot1.Date.Returns(DateOnly.FromDateTime(DateTime.Now));

            ITimeSlot mockTimeSlot2 = Substitute.For<ITimeSlot>();
            mockTimeSlot2.Date.Returns(DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

            var reservation1 = new Reservation
            {
                Boat = mockBoat,
                TimeSlot = mockTimeSlot1,
                User = mockUser
            };

            var reservation2 = new Reservation
            {
                Boat = mockBoat,
                TimeSlot = mockTimeSlot2,
                User = mockUser
            };

            IsValidReservationDate(reservation1, reservation2).ShouldBeFalse(
                         "Reservation should not be allowed within minimum 2 days.");
        }

        // mock methods for business logic checks
        private static bool CheckBoatAvailability(Reservation reservation1, Reservation reservation2)
        {
            return !(reservation1.BoatId == reservation2.BoatId &&
                     reservation1.TimeSlotId == reservation2.TimeSlotId);
        }

        private static bool IsValidReservationDate(Reservation reservation1, Reservation reservation2)
        {
            return (reservation2.TimeSlot.Date.DayNumber - reservation1.TimeSlot.Date.DayNumber)
                   >= Reservation.MinDaysBetweenReservation;
        }
    }
}
