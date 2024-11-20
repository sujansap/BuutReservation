using Rise.Domain.Boats;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;

namespace Rise.Domain.Reservations
{
    public interface IReservation : IEntity
    {
        public int BoatId { get; set; }
        public Boat Boat { get; set; }

        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public int UserId { get; }
        public User User { get; }
    }
}