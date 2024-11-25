using Rise.Domain.Reservations;

namespace Rise.Domain.Boats
{
    public class Boat : Entity
    {
        private string _personalName = default!;

        public required string PersonalName
        {
            get => _personalName;
            set => _personalName = Guard.Against.NullOrWhiteSpace(value, nameof(PersonalName)).Trim();
        }

        private readonly List<Battery> _batteries = new();
        private readonly List<Reservation> _reservations = new();

        public IReadOnlyCollection<Battery> Batteries => _batteries.AsReadOnly();
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        public Battery? GetAvailableBatteryForDate(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return _batteries
                .OrderBy(b => b.UsageCount)
                .ThenBy(b => b.LastUsedAt ?? DateTime.MinValue)
                .FirstOrDefault(b => b.IsAvailableForDate(date, startTime, endTime));
        }

        internal void AddBattery(Battery battery)
        {
            Guard.Against.Null(battery, nameof(battery));
            _batteries.Add(battery);
        }

        internal void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Add(reservation);
        }
    }
}
