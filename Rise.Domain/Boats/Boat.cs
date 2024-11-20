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

        public ICollection<IReservation> Reservations { get; } = [];

        public ICollection<Battery> Batteries { get; } = [];

    }
}