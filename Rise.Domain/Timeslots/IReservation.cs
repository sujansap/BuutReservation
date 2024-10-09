using Rise.Domain.Boats;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;

namespace Rise.Domain.Reservations
{
    public interface IReservation
    {

        public int AmountAdults
        {
            get;
            set;
        }

        public int AmountChildren
        {
            get;
            set;
        }

        public int AmountPets
        {
            get;
            set;
        }

        public int BoatId { get; set; }
        public IBoat Boat { get; set; }

        public int BatteryId { get; set; }
        public IBattery Battery { get; set; }

        public int TimeSlotId { get; set; }
        public ITimeSlot TimeSlot { get; set; }

        public ICollection<IUser> Users { get; }
    }
}