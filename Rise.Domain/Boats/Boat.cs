using Rise.Domain.Reservations;
using Rise.Domain.TimeSlots;

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

        public Battery? FindAvailableBattery(TimeSlot timeSlot, DateTime currentTime)
        {
            Guard.Against.Null(timeSlot);
            Guard.Against.Null(currentTime);

            IEnumerable<Battery> compatibleBatteries = batteries
                .OrderBy(b => b.UsageCount)
                .ThenBy(b => b.Type);

            return compatibleBatteries.FirstOrDefault(
                b => b.IsAvailableForTimeSlot(timeSlot));
        }

        public List<Reservation> AssignBatteriesToReservations(DateTime now)
        {
            DateOnly today = DateOnly.FromDateTime(now);

            return reservations
                .Where(r =>
                        !r.IsDeleted &&
                        today <= r.TimeSlot.Date &&
                        r.TimeSlot.Date <= today.AddDays(Reservation.MinDaysBetweenReservation)
                    )
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .Where(r => r.Battery is null)
                .Select(reservation =>
                {
                    var battery = FindAvailableBattery(reservation.TimeSlot, now);
                    if (battery is not null)
                    {
                        return reservation.AssignBattery(battery);
                    }
                    return null;
                })
                .Where(r => r is not null)
                .ToList()!;
        }

    }
}