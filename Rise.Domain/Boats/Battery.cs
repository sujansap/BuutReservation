using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity
    {
        private string _type = default!;
        private readonly List<Reservation> _reservations = [];
        private int _usageCount;
        private DateTime? _lastUsedAt;

        public int UsageCount => _usageCount;
        public DateTime? LastUsedAt => _lastUsedAt;
        public int? CurrentHolderId { get; private set; }
        public User? CurrentHolder { get; private set; }

        private User _mentor = default!;
        public required User Mentor
        {
            get => _mentor;
            set => _mentor = Guard.Against.Null(value, nameof(Mentor), "Mentor cannot be null or empty");
        }

        public int BoatId { get; set; }
        public required Boat Boat { get; set; }
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

        internal void UpdateUsageStats()
        {
            _usageCount++;
            _lastUsedAt = DateTime.UtcNow;
        }

        public void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Add(reservation);
        }

        internal void RemoveReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Remove(reservation);
        }

        public bool IsAvailableForDate(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            if (Reservations.Any(r => r.Battery?.Id == Id))
                return false;

            var reservationsOnDate = Reservations
                .Where(r => r.TimeSlot.Date == date)
                .OrderBy(r => r.TimeSlot.Start)
                .ToList();

            if (reservationsOnDate.Count == 0)
                return true;

            foreach (var reservation in reservationsOnDate)
            {
                var timeDiffToStart = (startTime.ToTimeSpan() - reservation.TimeSlot.End.ToTimeSpan()).TotalHours;
                var timeDiffFromEnd = (reservation.TimeSlot.Start.ToTimeSpan() - endTime.ToTimeSpan()).TotalHours;

                if (timeDiffToStart < 4 && timeDiffFromEnd < 4)
                {
                    return false;
                }
            }

            return true;
        }

        public void AssignToHolder(User? user)
        {
            if (user is null)
            {
                CurrentHolder = Mentor;
                CurrentHolderId = Mentor.Id;
                return;
            }

            Guard.Against.Null(user, nameof(user));
            CurrentHolder = user;
            CurrentHolderId = user.Id;
        }

        public bool HasSufficientChargingTime(DateTime currentTime)
        {
            if (LastUsedAt == null) return true;

            var hoursSinceLastUse = (currentTime - LastUsedAt.Value).TotalHours;
            return hoursSinceLastUse >= 4;
        }

        public bool IsAvailableFor(DateOnly date, TimeOnly startTime, TimeOnly endTime, DateTime currentTime)
        {
            return HasSufficientChargingTime(currentTime) && IsAvailableForDate(date, startTime, endTime);
        }

        public static IEnumerable<Battery> GetCompatibleBatteriesForBoat(
            IEnumerable<Battery> batteries,
            int boatId)
        {
            Guard.Against.Null(batteries, nameof(batteries));
            Guard.Against.NegativeOrZero(boatId, nameof(boatId));

            return batteries
                .Where(b => b.BoatId == boatId)
                .OrderBy(b => b.UsageCount)
                .ThenBy(b => b.LastUsedAt ?? DateTime.MinValue);
        }

        public static async Task<Battery?> FindAvailableBatteryAsync(
            IEnumerable<Battery> compatibleBatteries,
            DateOnly date,
            TimeOnly start,
            TimeOnly end,
            DateTime currentTime)
        {
            Guard.Against.Null(compatibleBatteries, nameof(compatibleBatteries));

            return await Task.Run(() => compatibleBatteries
                .FirstOrDefault(b =>
                    b.HasSufficientChargingTime(currentTime) &&
                    b.IsAvailableForDate(date, start, end)));
        }

        public static async Task AssignBatteriesToReservationsAsync(
            IEnumerable<Reservation> reservations,
            IEnumerable<Battery> batteries,
            TimeInfo timeInfo)
        {
            var reservationsByDate = reservations
                .GroupBy(r => r.TimeSlot.Date)
                .OrderBy(g => g.Key);

            foreach (var dateGroup in reservationsByDate)
            {
                foreach (var reservation in dateGroup.OrderBy(r => r.TimeSlot.Start))
                {
                    if (reservation.Battery is null)
                    {
                        var compatibleBatteries = GetCompatibleBatteriesForBoat(
                            batteries, reservation.BoatId);

                        var availableBattery = await FindAvailableBatteryAsync(
                            compatibleBatteries,
                            reservation.TimeSlot.Date,
                            reservation.TimeSlot.Start,
                            reservation.TimeSlot.End,
                            timeInfo.Now);

                        if (availableBattery is not null)
                        {
                            reservation.Battery = availableBattery;
                            availableBattery.AssignToHolder(reservation.User);
                        }
                    }
                }
            }
        }

        public static void HandleCompletedReservations(
            IEnumerable<Reservation> completedReservations)
        {
            foreach (var reservation in completedReservations)
            {
                var lastUser = reservation.User;
                var battery = reservation.Battery!;
                battery.UpdateUsageStats();
                reservation.Battery = null;
                battery.AssignToHolder(lastUser);
            }
        }
    }
}
