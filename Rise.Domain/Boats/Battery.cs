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

        public int UsageCount { get; private set; }
        public DateTime? LastUsedAt { get; private set; }

        // TODO remove boat ids
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
            UsageCount++;
            LastUsedAt = DateTime.UtcNow;
        }

        internal void RemoveReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Remove(reservation);
        }

        public bool IsAvailableForDate(DateOnly targetDate, TimeOnly targetStart)
        {
            const int ChargingHours = 4;
            const int ReservationHours = 3;
            
            var targetDateTime = targetDate.ToDateTime(targetStart);
            var targetEnd = targetDateTime.AddHours(ReservationHours);
            var targetEndWithCharging = targetEnd.AddHours(ChargingHours);

            // Debug information
            Console.WriteLine($"Checking availability for Battery {Id} for date {targetDate} start {targetStart}");
            Console.WriteLine($"Target period: {targetDateTime} to {targetEnd} (with charging until {targetEndWithCharging})");
            
            foreach (var reservation in Reservations)
            {
                var reservationDateTime = reservation.TimeSlot.Date.ToDateTime(reservation.TimeSlot.Start);
                var reservationEnd = reservationDateTime.AddHours(ReservationHours);
                var reservationEndWithCharging = reservationEnd.AddHours(ChargingHours);

                Console.WriteLine($"Existing reservation: {reservationDateTime} to {reservationEnd} (with charging until {reservationEndWithCharging})");

                // Check if there's any overlap
                if ((targetDateTime < reservationEndWithCharging && targetEnd > reservationDateTime) ||
                    (targetEnd < reservationEndWithCharging && targetEndWithCharging > reservationDateTime))
                {
                    Console.WriteLine($"Overlap detected - Battery {Id} not available");
                    return false;
                }
            }
            
            Console.WriteLine($"Battery {Id} is available");
            return true;
        }
    }
}