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
    }
}