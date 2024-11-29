using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity
    {
        private string _type = default!;
        private readonly List<Reservation> _reservations = new();
        private int _usageCount;
        private DateTime? _lastUsedAt;

        public int UsageCount => _usageCount;
        public DateTime? LastUsedAt => _lastUsedAt;
        public int? CurrentHolderId { get; private set; }
        public User? CurrentHolder { get; private set; }
        public int MentorId { get; set; }
        public User Mentor { get; set; } = null!;
        public int BoatId { get; set; }
        public required Boat Boat { get; set; }
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

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
            var reservationsOnDate = Reservations
                .Where(r => r.TimeSlot.Date == date)
                .OrderBy(r => r.TimeSlot.Start)
                .ToList();

            if (!reservationsOnDate.Any())
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
    }
}
