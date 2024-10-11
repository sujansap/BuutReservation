// using Rise.Domain.Reservations;

using Rise.Domain.Reservations;

namespace Rise.Domain.Users
{
    /// <summary>
    /// User base class
    /// </summary>
    public class User: Entity, IUser
    {
        private string _familyName = default!;


        public required string FamilyName {
            get => _familyName;
            set => _familyName = Guard.Against.NullOrWhiteSpace(value);
        }

        public ICollection<IReservation> Reservations { get; } = [];
    }
}