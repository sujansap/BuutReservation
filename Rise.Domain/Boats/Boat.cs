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
        private readonly List<Reservation> reservations = [];
        public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();

        private readonly List<Battery> batteries = [];

        public IReadOnlyList<Battery> Batteries => batteries.AsReadOnly();

        public Battery? GetAvailableBatteryForDate(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return batteries
                .OrderBy(b => b.UsageCount)
                .ThenBy(b => b.LastUsedAt ?? DateTime.MinValue)
                .FirstOrDefault(b => b.IsAvailableForDate(date, startTime, endTime));
        }

        public void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            reservations.Add(reservation);
        }

        public void AddBattery(Battery battery)
        {
            Guard.Against.Null(battery, nameof(battery));
            batteries.Add(battery);
        }

    }
}
