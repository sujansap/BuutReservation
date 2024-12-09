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

        public void AssignBatteriesToReservations(DateTime now)
        {
            DateOnly today = DateOnly.FromDateTime(now);
            var reservationsByDate = reservations
                .Where(r => !r.IsDeleted && today <= r.TimeSlot.Date && r.TimeSlot.Date <= today.AddDays(Reservation.MinDaysBetweenReservation))
                .GroupBy(r => r.TimeSlot.Date)
                .OrderBy(g => g.Key);

            foreach (var dateGroup in reservationsByDate)
            {
                foreach (var reservation in dateGroup.OrderBy(r => r.TimeSlot.Start))
                {
                    if (reservation.Battery is null)
                        AssignBatteryToReservation(reservation, now);
                    else
                        continue;
                }
            }
        }

        private void AssignBatteryToReservation(Reservation reservation, DateTime now)
        {
            Battery? availableBattery = FindAvailableBattery(reservation.TimeSlot, now);
            if (availableBattery is not null)
            {
                reservation.AssignBattery(availableBattery);
            }
        }

    }
}