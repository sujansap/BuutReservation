using Rise.Domain.Reservations;

namespace Rise.Domain.Boats
{

    public class Boat : Entity, IBoat
    {
        /// <summary>
        /// Personal name of the boat. Not to be confused with the type/class name. e.x
        /// </summary>    
        private string _personalName = default!;

        public required string PersonalName
        {
            get => _personalName;
            set => _personalName = Guard.Against.NullOrWhiteSpace(value);
        }

        private readonly DateTime _todayProvider;

        /// <summary>
        /// Start moment of when the boat is out of order and cannot be used.
        /// </summary>
        private DateTime? _startOutOfOrder = null;

        public DateTime? StartOutOfOrder
        {
            get => _startOutOfOrder;
            protected set => _startOutOfOrder = value.HasValue ? Guard.Against.OutOfRange(value.Value, "StartOutOfOrder", _todayProvider.Date, new DateTime(9999, 12, 31), "Start out of order date cannot lay in the past") : null;
        }


        /// <summary>
        /// End moment of when the boat is out of order and cannot be used.
        /// </summary>
        private DateTime? _endOutOfOrder = null;

        public DateTime? EndOutOfOrder
        {
            get => _endOutOfOrder;
            protected set => _endOutOfOrder = _startOutOfOrder.HasValue && value.HasValue ? Guard.Against.OutOfRange(value.Value, "EndOutOfOrder", _startOutOfOrder.Value, new DateTime(9999, 12, 31), "End out of order date cannot be before the start") : null;
        }

        private int _maximumAdults;
        public int MaximumAdults
        {
            get => _maximumAdults;
            set => _maximumAdults = Guard.Against.NegativeOrZero(value, "MaximumAdults", "Maximum amount of adults cannot be negative or zero");
        }

        private int _maximumChildren;
        public int MaximumChildren
        {
            get => _maximumChildren;
            set => _maximumChildren = Guard.Against.Negative(value, "MaximumChildren", "Maximum amount of children cannot be negative");
        }

        private int _maximumPets;
        public int MaximumPets
        {
            get => _maximumPets;
            set => _maximumPets = Guard.Against.Negative(value, "MaximumPets", "Maximum amount of adults cannot be negative");
        }

        public ICollection<IBattery> Batteries { get; } = [];

        public ICollection<IReservation> Reservations { get; } = [];

        // TODO add relation to meter and peter users

        public Boat() : base()
        {
            _todayProvider = DateTime.UtcNow;
        }

        public Boat(DateTime todayProvider) : base()
        {
            _todayProvider = todayProvider;
        }

        public void DefineOutOfOrderPeriod(DateTime? startOutOfOrder, DateTime? endOutOfOrder = null)
        {
            StartOutOfOrder = startOutOfOrder;
            EndOutOfOrder = endOutOfOrder;
        }

        public void RemoveOutOfOrderPeriod()
        {
            DefineOutOfOrderPeriod(null);
        }


        public bool IsInOutOfOrderPeriod(DateTime date)
        {
            if (_startOutOfOrder.HasValue)
            {
                if (DateTime.Compare(_startOutOfOrder.Value, date) <= 0)
                {
                    return !_endOutOfOrder.HasValue || DateTime.Compare(date, _endOutOfOrder.Value) <= 0;
                }

            }
            return false;
        }

    }
}