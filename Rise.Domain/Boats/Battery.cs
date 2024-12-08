using Rise.Domain.Reservations;
using Rise.Domain.TimeSlots;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity
    {
        private const int rechargeBufferHours = 4;
        private string _type = default!;
        private readonly List<Reservation> _reservations = [];
        private int _usageCount;
        private DateTime? _lastUsedAt;

        public int UsageCount => _usageCount;
        public DateTime? LastUsedAt => _lastUsedAt;
        public User? CurrentHolder { get; private set; }

        private User _mentor = default!;
        public required User Mentor
        {
            get => _mentor;
            set => _mentor = Guard.Against.Null(value, nameof(Mentor), "Mentor cannot be null or empty");
        }

        public required Boat Boat { get; set; }
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

        private void UpdateUsageStats(DateTime? lastUsed)
        {
            _usageCount++;
            _lastUsedAt = lastUsed ?? DateTime.UtcNow;
        }

        public void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Add(reservation);
        }

        public void RemoveReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Remove(reservation);
        }

        public bool IsAvailableForTimeSlot(TimeSlot timeSlot)
        {
            int above = 24 - rechargeBufferHours;

            return !Reservations
                .Where(r => r.TimeSlot.Date == timeSlot.Date)
                .Any(r =>
                {
                    int hourDifference = (r.TimeSlot.End - timeSlot.Start).Hours;
                    return hourDifference < rechargeBufferHours || above < hourDifference;
                });
        }

        public void AssignToHolder(User? user, DateTime? lastUsed)
        {
            CurrentHolder = user is not null ? user : Mentor;
            UpdateUsageStats(lastUsed);
        }

        public bool HasSufficientChargingTime(DateTime currentTime)
        {
            if (LastUsedAt == null) return true;

            double hoursSinceLastUse = (currentTime - LastUsedAt.Value).TotalHours;
            return hoursSinceLastUse >= rechargeBufferHours;
        }
    }
}
