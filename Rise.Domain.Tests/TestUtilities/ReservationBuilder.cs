using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.TestUtilities
{
    public class ReservationBuilder
    {
        // TODO valid boats en stuff
        public static readonly Boat ValidBoat = new BoatBuilder().Build();
        private Boat boat = ValidBoat;

        public static readonly TimeSlot ValidTimeSlot = new TimeSlotBuilder().Build();
        private TimeSlot timeSlot = ValidTimeSlot;

        public static readonly User ValidUser = new UserBuilder().Build();
        private User user = ValidUser;

        public ReservationBuilder WithBoat(Boat boat)
        {
            this.boat = boat;
            return this;
        }

        public ReservationBuilder WithTimeSlot(TimeSlot timeSlot)
        {
            this.timeSlot = timeSlot;
            return this;
        }

        public ReservationBuilder WithUser(User user)
        {
            this.user = user;
            return this;
        }

        public Reservation Build()
        {
            return new()
            {
                Boat = boat,
                TimeSlot = timeSlot,
                User = user,
            };
        }
    }

}