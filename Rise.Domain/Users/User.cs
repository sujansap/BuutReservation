// using Rise.Domain.Reservations;

using Rise.Domain.Notifications;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;

namespace Rise.Domain.Users
{
    /// <summary>
    /// User base class
    /// </summary>
    public class User : Entity
    {
        private string _email = default!;
        private string _firstName = default!;
        private string _familyName = default!;
        private string _phoneNumber = default!;
        private UserAddress _address = default!;

        public required string Email
        {
            get => _email;
            set
            {
                Guard.Against.NullOrWhiteSpace(value, nameof(Email));
                Guard.Against.LengthOutOfRange(value, 1, 100);
                Guard.Against.InvalidFormat(value, nameof(Email), "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
                value = value.Trim();
                _email = value;
            }
        }

        public required string FirstName
        {
            get => _firstName;
            set
            {
                Guard.Against.NullOrWhiteSpace(value, nameof(FirstName));
                Guard.Against.LengthOutOfRange(value, 1, 100, nameof(FirstName));
                value = value.Trim();
                _firstName = value;
            }
        }
        public required string FamilyName
        {
            get => _familyName;
            set
            {
                Guard.Against.NullOrWhiteSpace(value, nameof(FamilyName));
                Guard.Against.LengthOutOfRange(value, 1, 100, nameof(FamilyName));
                value = value.Trim();
                _familyName = value;
            }
        }
        public required string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                Guard.Against.NullOrWhiteSpace(value, nameof(PhoneNumber));
                Guard.Against.LengthOutOfRange(value, 1, 100, nameof(PhoneNumber));
                Guard.Against.InvalidFormat(value, nameof(PhoneNumber), "^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");
                value = value.Trim();
                _phoneNumber = value;
            }
        }

        public required UserAddress Address
        {
            get => _address;
            set
            {
                Guard.Against.NullOrWhiteSpace(value.Street, nameof(value.Street));
                Guard.Against.LengthOutOfRange(value.Street, 1, 200, nameof(value.Street));

                Guard.Against.NullOrWhiteSpace(value.Number, nameof(value.Number));
                Guard.Against.LengthOutOfRange(value.Number, 1, 200, nameof(value.Number));

                Guard.Against.NullOrWhiteSpace(value.City, nameof(value.City));
                Guard.Against.LengthOutOfRange(value.City, 1, 200, nameof(value.City));

                Guard.Against.NullOrWhiteSpace(value.PostalCode, nameof(value.PostalCode));
                Guard.Against.LengthOutOfRange(value.PostalCode, 1, 100, nameof(value.PostalCode));

                Guard.Against.NullOrWhiteSpace(value.Country, nameof(value.Country));
                Guard.Against.LengthOutOfRange(value.Country, 1, 100, nameof(value.Country));
                _address = value;
            }
        }

        public class UserAddress
        {
            private string _street = default!;
            private string _number = default!;
            private string _city = default!;
            private string _postalCode = default!;
            private string _country = default!;
            public required string Street { get => _street; set => _street = value; }
            public required string Number { get => _number; set => _number = value; }
            public required string City { get => _city; set => _city = value; }
            public required string PostalCode { get => _postalCode; set => _postalCode = value; }
            public required string Country { get => _country; set => _country = value; }
        }

        private readonly List<Reservation> reservations = [];
        public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();
        private readonly List<Battery> guardedBatteries = [];
        public IReadOnlyList<Battery> GuardedBatteries => guardedBatteries.AsReadOnly();
        private readonly List<Notification> notifications = [];
        public IReadOnlyList<Notification> Notifications => notifications.AsReadOnly();
    }
}