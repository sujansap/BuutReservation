using Shouldly;
using Rise.Domain.Reservations;
using Rise.Domain.Tests.TestUtilities;

namespace Rise.Domain.Tests.Timeslots
{
    public class ReservationShould
    {
        [Fact]
        public void BeCreatedWithUser()
        {
            Reservation reservation = new ReservationBuilder().Build();

            // Act & Assert
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
