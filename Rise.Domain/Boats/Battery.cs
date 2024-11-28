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

        public int? CurrentHolderId { get; private set; }
        public User? CurrentHolder { get; private set; }

        public int MentorId { get; set; }
        public User Mentor { get; set; } = null!;

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

        public int BoatId { get; set; }
        public required Boat Boat { get; set; }

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

        public void AssignToHolder(User? user)
        {
            if (user == null)
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
            // First check if battery has sufficient charging time
            if (!HasSufficientChargingTime(currentTime))
                return false;

            // Then check if battery is available for the specific time slot
            return IsAvailableForDate(date, startTime, endTime);
        }
    }
}
