using Shouldly;
using Rise.Domain.Reservations;
using Rise.Domain.Tests.TestUtilities;

namespace Rise.Domain.Tests.Reservations
{
    public class ReservationShould
    {
        [Fact]
        public void BeCreatedWithUser()
        {
            Reservation reservation = new ReservationBuilder().Build();

            
            reservation.Boat.ShouldBe(ReservationBuilder.ValidBoat);
            reservation.TimeSlot.ShouldBe(ReservationBuilder.ValidTimeSlot);
            reservation.User.ShouldBe(ReservationBuilder.ValidUser);
        }

        [Fact]
        public void NotBeCreatedIfBoatIsAlreadyReservedForTimeSlot()
        {

            Reservation reservation1 = new ReservationBuilder().Build();

            Reservation reservation2 = new ReservationBuilder()
            .WithUser(
                new UserBuilder()
                    .WithFamilyName("Other user")
                    .Build()
            )
            .Build();

            CheckBoatAvailability(reservation1, reservation2).ShouldBeFalse("Boat should not be available for the same time slot.");
        }

        [Fact]
        public void ShouldEnforceMinimumDaysBetweenReservations()
        {
            Reservation reservation1 = new ReservationBuilder().Build();

            Reservation reservation2 = new ReservationBuilder()
            .WithTimeSlot(
                new TimeSlotBuilder()
                .WithDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
                .Build()
            )
            .Build();

            IsValidReservationDate(reservation1, reservation2).ShouldBeFalse(
                         "Reservation should not be allowed within minimum 2 days.");
        }

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

        [Fact]
        public void CancelReservationSuccessfully_WhenValid()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
                        .Build()
                )
                .Build();

            
            reservation.Cancel();

            
            reservation.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void ThrowException_WhenAlreadyCancelled()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(4)))
                        .Build()
                )
                .Build();

            reservation.Cancel();

            
            Should.Throw<InvalidOperationException>(() => reservation.Cancel())
                .Message.ShouldBe("The reservation is already canceled.");
        }

        [Fact]
        public void ThrowException_WhenCancellationWithinTwoDays()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1))) // Less than 2 days
                        .Build()
                )
                .Build();

            Should.Throw<InvalidOperationException>(() => reservation.Cancel())
                .Message.ShouldBe("Reservations can only be canceled at least 2 days before the reservation date.");
        }

        [Fact]
        public void NotThrowException_WhenCancellationExactlyTwoDaysBefore()
        {
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2))) // Exactly 2 days
                        .Build()
                )
                .Build();

            reservation.Cancel();

            reservation.IsDeleted.ShouldBeTrue();
        }

    }
}
