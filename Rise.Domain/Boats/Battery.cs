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

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }
        public required Boat Boat { get; set; }

        public required User Mentor { get; set; }

    }
}