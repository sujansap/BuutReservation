using Rise.Domain.Boats;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;

namespace Rise.Domain.Reservations
{
    public interface IReservation : IEntity
    {
        public int BoatId { get; set; }
        public IBoat Boat { get; set; }

        public int TimeSlotId { get; set; }
        public ITimeSlot TimeSlot { get; set; }

        public int UserId { get; }
        public IUser User { get; }
    }
}