using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity
    {
        /// <summary>
        /// The type of battery
        /// </summary>
        private string _type = default!;

        private readonly List<Reservation> _reservations = [];

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

        // TODO remove boat id
        public int BoatId { get; set; }
        public required Boat Boat { get; set; }

        // TODO remove mentor id
        public int MentorId { get; set; }
        public required User Mentor { get; set; }

        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        internal void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Add(reservation);
        }

        internal void RemoveReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Remove(reservation);
        }

        public bool IsAvailableForDate(DateOnly targetDate, TimeOnly targetStart)
        {
            const int ChargingHours = 4;
            
            foreach (var reservation in Reservations)
            {
                // Check reservations within 24 hours
                if (Math.Abs(reservation.TimeSlot.Date.DayNumber - targetDate.DayNumber) <= 1)
                {
                    var reservationDateTime = reservation.TimeSlot.Date.ToDateTime(reservation.TimeSlot.Start);
                    var reservationEndWithCharging = reservation.TimeSlot.Date.ToDateTime(reservation.TimeSlot.End)
                        .AddHours(ChargingHours);
                    var targetDateTime = targetDate.ToDateTime(targetStart);
                    var targetEndWithCharging = targetDate.ToDateTime(targetStart).AddHours(3) // Assuming 3 hour slots
                        .AddHours(ChargingHours);

                    // Check if either the start or end (including charging time) overlaps
                    if (targetDateTime <= reservationEndWithCharging && 
                        targetEndWithCharging >= reservationDateTime)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}