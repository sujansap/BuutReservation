using System.Collections;
using Rise.Domain.Boats;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;

namespace Rise.Domain.Reservations
{
    public class Reservation : Entity, IReservation
    {
        public readonly static int MinDaysBetweenReservation = 2;
        public int BoatId { get; set; }
        public required Boat Boat { get; set; }

        public int TimeSlotId { get; set; }
        public required TimeSlot TimeSlot { get; set; }

        public int UserId { get; set; }
        public required User User { get; set; }

        public Reservation()
        {
        }

    }
}