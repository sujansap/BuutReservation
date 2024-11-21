using Rise.Domain.Boats;
using Rise.Domain.Timeslots;
using Rise.Domain.Users;

namespace Rise.Domain.Reservations
{
    public class Reservation : Entity
    {
        public readonly static int MinDaysBetweenReservation = 2;

        // TODO remove boat id
        public int BoatId { get; set; }
        public required Boat Boat { get; set; }

        // TODO remove time slot id
        public int TimeSlotId { get; set; }
        public required TimeSlot TimeSlot { get; set; }

        // TODO remove user id
        public int UserId { get; set; }
        public required User User { get; set; }

        private Battery? _battery;
        public Battery? Battery 
        { 
            get => _battery;
            set
            {
                _battery?.RemoveReservation(this);
                _battery = value;
                if (_battery is not null)
                {
                    BatteryId = _battery.Id;
                    _battery.AddReservation(this);
                }
                else
                {
                    BatteryId = null;
                }
            }
        }
        
        public int? BatteryId { get; private set; }
    }
}