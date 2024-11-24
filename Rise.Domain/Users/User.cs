// using Rise.Domain.Reservations;

using Rise.Domain.Notifications;
using Rise.Domain.Reservations;

namespace Rise.Domain.Users
{
    /// <summary>
    /// User base class
    /// </summary>
    public class User : Entity
    {
        private string _familyName = default!;

        public required string FamilyName
        {
            get => _familyName;
            set => _familyName = Guard.Against.NullOrWhiteSpace(value, nameof(FamilyName)).Trim();
        }

        public ICollection<Reservation> Reservations { get; } = [];
        private readonly List<Notification> notifications = [];
        public IReadOnlyList<Notification> Notifications => notifications.AsReadOnly();
    }
}