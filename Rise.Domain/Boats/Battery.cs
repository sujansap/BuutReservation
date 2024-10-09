using Rise.Domain.Reservations;

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

        private double _maximumCapacity = 100.0;

        public double MaximumCapacity
        {
            get => _maximumCapacity;
            set => _maximumCapacity = Guard.Against.OutOfRange(value, "MaximumCapacity", 1.0, 100.0, "Maximum capacity needs to be between 1 and 100 %");
        }

        private double _currentLoad = 100.0;

        public double CurrentLoad
        {
            get => _currentLoad;
            set => _currentLoad = Guard.Against.OutOfRange(value, "CurrentLoad", 0.0, MaximumCapacity, "Battery capacity must be between 0 and 100 %");
        }

        private bool _outOfOrder = false;

        public bool OutOfOrder
        {
            get => _outOfOrder;
            set => _outOfOrder = Guard.Against.Null(value, "OutOfOrder");
        }

        public int? BoatId { get; set; }
        public IBoat? Boat { get; set; } = null!;
        public ICollection<IReservation> Reservations
        {
            get;
        } = [];
    }
}