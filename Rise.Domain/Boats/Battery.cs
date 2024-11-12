using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity, IBattery
    {
        /// <summary>
        /// The type of battery
        /// </summary>
        private string _type = default!;

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrEmpty(value, "Type", "Battery type cannot be null or zero");
        }

        public int BoatId { get; set; }
        public required IBoat Boat { get; set; }

        public int MentorId { get; set; }
        public required IUser Mentor { get; set; }


        public ICollection<IReservation> Reservations
        {
            get;
        } = [];

    }
}