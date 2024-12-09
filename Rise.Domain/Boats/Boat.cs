using Rise.Domain.Reservations;

namespace Rise.Domain.Boats
{

    public class Boat : Entity
    {
        /// <summary>
        /// Personal name of the boat. Not to be confused with the type/class name. e.x
        /// </summary>    
        private string _personalName = default!;

        public required string PersonalName
        {
            get => _personalName;
            set => _personalName = Guard.Against.NullOrWhiteSpace(value, nameof(PersonalName)).Trim();
        }

        /// <summary>
        /// Indicates whether the boat is available for reservations
        /// </summary>
        private bool _isAvailable = true;

        public bool IsAvailable
        {
            get => _isAvailable;
            set => _isAvailable = Guard.Against.Null(value, nameof(IsAvailable), "Availability status cannot be null");
        }
        private readonly List<Reservation> reservations = [];
        public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();

        private readonly List<Battery> batteries = [];

        public IReadOnlyList<Battery> Batteries => batteries.AsReadOnly();

        public void AddReservation(Reservation reservation)
        {
            // TODO add logic for making reservation
            reservations.Add(reservation);
        }

        public void AddBattery(Battery battery)
        {
            // TODO add logic for adding battery
            batteries.Add(battery);
        }

        /// <summary>
        /// Updates the availability of the boat
        /// </summary>
        /// <param name="isAvailable">New availability status</param>
        public void ChangeAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }



    }
}