using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity
    {
        private string _type = default!;

        private readonly List<Reservation> _reservations = new();

        private int _usageCount;
        public int UsageCount => _usageCount;

        private DateTime? _lastUsedAt;
        public DateTime? LastUsedAt => _lastUsedAt;

        private int? _currentHolderId;
        public int? CurrentHolderId => _currentHolderId;

        public User? CurrentHolder { get; private set; }

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

        public int BoatId { get; set; }
        public required Boat Boat { get; set; }

        public int MentorId { get; set; }
        public required User Mentor { get; set; }

        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        internal void IncrementUsage()
        {
            _usageCount++;
            _lastUsedAt = DateTime.UtcNow;
        }

        public void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Add(reservation);
            IncrementUsage();
        }

        internal void RemoveReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Remove(reservation);
        }

        public bool IsAvailableForDate(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            // Get all reservations for this battery on the given date
            var reservationsOnDate = Reservations
                .Where(r => r.TimeSlot.Date == date)
                .OrderBy(r => r.TimeSlot.Start)
                .ToList();

            if (!reservationsOnDate.Any())
                return true;

            // Check 4-hour gap requirement between reservations
            foreach (var reservation in reservationsOnDate)
            {
                // Calculate time differences for both start and end times
                var timeDiffToStart = (startTime.ToTimeSpan() - reservation.TimeSlot.End.ToTimeSpan()).TotalHours;
                var timeDiffFromEnd = (reservation.TimeSlot.Start.ToTimeSpan() - endTime.ToTimeSpan()).TotalHours;

                // If there's any overlap or less than 4 hours gap, battery is not available
                if (timeDiffToStart < 4 && timeDiffFromEnd < 4)
                {
                    return false;
                }
            }

            return true;
        }

        public void AssignToHolder(User user)
        {
            Guard.Against.Null(user, nameof(user));
            CurrentHolder = user;
            _currentHolderId = user.Id;
        }

        public bool HasSufficientChargingTime(DateTime currentTime)
        {
            if (LastUsedAt == null) return true;
            
            var hoursSinceLastUse = (currentTime - LastUsedAt.Value).TotalHours;
            return hoursSinceLastUse >= 4;
        }

        public static Battery? GetBestAvailableBattery(IEnumerable<Battery> batteries, DateOnly date, TimeOnly startTime, TimeOnly endTime, DateTime currentTime)
        {
            // Filter batteries that are available for the given time slot
            var availableBatteries = batteries
                .Where(b => b.IsAvailableForDate(date, startTime, endTime))
                .Where(b => b.HasSufficientChargingTime(currentTime))
                .ToList();

            if (!availableBatteries.Any())
                return null;

            // Among available batteries, select the one with lowest usage count
            return availableBatteries
                .OrderBy(b => b.UsageCount)
                .First();
        }
    }
}
