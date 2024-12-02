using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Users
{
    public class UserShould
    {
        private const string ValidEmail = "test.user@domain.com";
        private const string ValidFirstName = "John";
        private const string ValidFamilyName = "Doe";
        private const string ValidPhoneNumber = "+1234567890";
        private static readonly User.UserAddress ValidAddress = new()
        {
            Street = "123 Main St.",
            Number = "4B",
            City = "Springfield",
            PostalCode = "12345",
            Country = "USA"
        };

        #region Email Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        public void NotBeCreatedWithInvalidEmail(string? email)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithEmail(email).Build();
            }).ParamName.ShouldBe("Email");
        }

        [Fact]
        public void NotBeCreatedWithTooLongEmail()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithEmail("a." + new string('b', 63) + "@b.co").Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        public void NotBeChangedToHaveInvalidEmail(string? email)
        {
            Action act = () =>
            {
                User user = new UserBuilder()
                    .Build();
                user.Email = email;
            };

            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Email");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongEmail()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Email = new string('a', 70) + "@b.co"; // Exceeds max length of 69
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidEmail()
        {
            var user = new UserBuilder().WithEmail(ValidEmail).Build();
            user.Email.ShouldBe(ValidEmail);
        }

        #endregion

        #region FirstName Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeCreatedWithInvalidFirstName(string? firstName)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFirstName(firstName).Build();
            }).ParamName.ShouldBe("FirstName");
        }

        public void NotBeCreatedWithTooLongFirstName()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFirstName(new string('a', 101)).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidFirstName(string? firstName)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.FirstName = firstName;
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("FirstName");
        }

        public void NotBeChangedToHaveTooLongFirstName()
        {
            Action act = () =>
           {
               User user = new UserBuilder().Build();
               user.FirstName = new string('a', 101);
           };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidFirstName()
        {
            var user = new UserBuilder().WithFirstName(ValidFirstName).Build();
            user.FirstName.ShouldBe(ValidFirstName);
        }

        #endregion

        #region FamilyName Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeCreatedWithInvalidFamilyName(string familyName)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFamilyName(familyName).Build();
            }).ParamName.ShouldBe("FamilyName");
        }

        [Fact]
        public void NotBeCreatedWithTooLongFamilyName()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFamilyName(new string('a', 101)).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidFamilyName(string? familyName)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.FamilyName = familyName;
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("FamilyName");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongFamilyName()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.FamilyName = new string('a', 101); // Exceeds max length of 100
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidFamilyName()
        {
            var user = new UserBuilder().WithFamilyName(ValidFamilyName).Build();
            user.FamilyName.ShouldBe(ValidFamilyName);
        }

        #endregion

        #region PhoneNumber Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-phone")]
        public void NotBeCreatedWithInvalidPhoneNumber(string phoneNumber)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithPhoneNumber(phoneNumber).Build();
            }).ParamName.ShouldBe("PhoneNumber");
        }

        [Fact]
        public void NotBeCreatedWithTooLongPhoneNumber()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithPhoneNumber(new string('1', 101)).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-phone-number")]
        public void NotBeChangedToHaveInvalidPhoneNumber(string? phoneNumber)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.PhoneNumber = phoneNumber;
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("PhoneNumber");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongPhoneNumber()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.PhoneNumber = new string('1', 101); // Exceeds max length of 100
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidPhoneNumber()
        {
            var user = new UserBuilder().WithPhoneNumber(ValidPhoneNumber).Build();
            user.PhoneNumber.ShouldBe(ValidPhoneNumber);
        }

        #endregion

        #region Address Tests

        [Theory]
        [InlineData(null, "123", "Springfield", "12345", "USA")] // Null Street
        [InlineData("", "123", "Springfield", "12345", "USA")] // Empty Street
        [InlineData("   ", "123", "Springfield", "12345", "USA")] // Whitespace Street
        [InlineData("123", null, "Springfield", "12345", "USA")] // Null Number
        [InlineData("123", "", "Springfield", "12345", "USA")] // Empty Number
        [InlineData("123", "   ", "Springfield", "12345", "USA")] // Whitespace Number
        [InlineData("123", "123", null, "12345", "USA")] // Null City
        [InlineData("123", "123", "", "12345", "USA")] // Empty City
        [InlineData("123", "123", "   ", "12345", "USA")] // Whitespace City
        [InlineData("123", "123", "Springfield", null, "USA")] // Null PostalCode
        [InlineData("123", "123", "Springfield", "", "USA")] // Empty PostalCode
        [InlineData("123", "123", "Springfield", "   ", "USA")] // Whitespace PostalCode
        [InlineData("123", "123", "Springfield", "12345", null)] // Null Country
        [InlineData("123", "123", "Springfield", "12345", "")] // Empty Country
        [InlineData("123", "123", "Springfield", "12345", "   ")] // Whitespace Country
        public void NotBeCreatedWithInvalidAddress(
            string street, string number, string city, string postalCode, string country)
        {
            var invalidAddress = new User.UserAddress
            {
                Street = street,
                Number = number,
                City = city,
                PostalCode = postalCode,
                Country = country
            };

            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithAddress(invalidAddress).Build();
            });
        }

        #region "Street"

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidStreet(string? street)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = street,
                    Number = "123",
                    City = "City",
                    PostalCode = "12345",
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Street");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongStreet()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = new string('S', 201), // Exceeds max length of 200
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongStreet()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = new string('S', 201),
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
                var user = new UserBuilder().WithAddress(address).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region Number

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidNumber(string? number)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "Street",
                    Number = number,
                    City = "City",
                    PostalCode = "12345",
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Number");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongNumber()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = new string('N', 201), // Exceeds max length of 200
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongNumber()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = new string('N', 201),
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
                var user = new UserBuilder().WithAddress(address).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region City

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidCity(string? city)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "Street",
                    Number = "123",
                    City = city,
                    PostalCode = "12345",
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("City");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongCity()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = new string('C', 201), // Exceeds max length of 200
                    PostalCode = "12345",
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongCity()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = new string('C', 201),
                    PostalCode = "12345",
                    Country = "USA"
                };
                var user = new UserBuilder().WithAddress(address).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region PostalCode

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidPostalCode(string? postalCode)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "Street",
                    Number = "123",
                    City = "City",
                    PostalCode = postalCode,
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("PostalCode");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongPostalCode()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = new string('P', 101), // Exceeds max length of 100
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongPostalCode()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = new string('P', 101),
                    Country = "USA"
                };
                var user = new UserBuilder().WithAddress(address).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region Country

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidCountry(string? country)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "Street",
                    Number = "123",
                    City = "City",
                    PostalCode = "12345",
                    Country = country
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Country");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongCountry()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = new string('C', 101) // Exceeds max length of 100
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongCountry()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = new string('C', 101)
                };
                var user = new UserBuilder().WithAddress(address).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        [Fact]
        public void BeCreatedWithValidAddress()
        {
            var user = new UserBuilder().WithAddress(ValidAddress).Build();
            user.Address.ShouldBe(ValidAddress);
        }

        #endregion

        #region Trimming Tests

        [Fact]
        public void TrimEmail()
        {
            var emailWithSpaces = $"   {ValidEmail}   ";
            var user = new UserBuilder().WithEmail(emailWithSpaces).Build();
            user.Email.ShouldBe(ValidEmail); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimFirstName()
        {
            var firstNameWithSpaces = $"   {ValidFirstName}   ";
            var user = new UserBuilder().WithFirstName(firstNameWithSpaces).Build();
            user.FirstName.ShouldBe(ValidFirstName); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimFamilyName()
        {
            var familyNameWithSpaces = $"   {ValidFamilyName}   ";
            var user = new UserBuilder().WithFamilyName(familyNameWithSpaces).Build();
            user.FamilyName.ShouldBe(ValidFamilyName); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimPhoneNumber()
        {
            var phoneNumberWithSpaces = $"   {ValidPhoneNumber}   ";
            var user = new UserBuilder().WithPhoneNumber(phoneNumberWithSpaces).Build();
            user.PhoneNumber.ShouldBe(ValidPhoneNumber); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressStreet()
        {
            var untrimmedStreet = $"   {ValidAddress.Street}   ";

            var user = new UserBuilder()
                .WithAddress(new User.UserAddress
                {
                    Street = untrimmedStreet,
                    Number = ValidAddress.Number,
                    City = ValidAddress.City,
                    PostalCode = ValidAddress.PostalCode,
                    Country = ValidAddress.Country
                })
                .Build();

            user.Address.Street.ShouldBe(ValidAddress.Street); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressNumber()
        {
            var trimmedStreet = "123 Main St.";
            var untrimmedStreet = "  123 Main St.  ";

            var user = new UserBuilder()
                .WithAddress(new User.UserAddress
                {
                    Street = untrimmedStreet,
                    Number = "4B",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                })
                .Build();

            user.Address.Street.ShouldBe(trimmedStreet);
        }

        [Fact]
        public void TrimAddressCity()
        {
            var untrimmedCity = $"   {ValidAddress.City}   ";

            var user = new UserBuilder()
                .WithAddress(new User.UserAddress
                {
                    Street = ValidAddress.Street,
                    Number = ValidAddress.Number,
                    City = untrimmedCity,
                    PostalCode = ValidAddress.PostalCode,
                    Country = ValidAddress.Country
                })
                .Build();

            user.Address.City.ShouldBe(ValidAddress.City); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressPostalCode()
        {
            var untrimmedPostalCode = $"   {ValidAddress.PostalCode}   ";

            var user = new UserBuilder()
                .WithAddress(new User.UserAddress
                {
                    Street = ValidAddress.Street,
                    Number = ValidAddress.Number,
                    City = ValidAddress.City,
                    PostalCode = untrimmedPostalCode,
                    Country = ValidAddress.Country
                })
                .Build();

            user.Address.PostalCode.ShouldBe(ValidAddress.PostalCode); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressCountry()
        {
            var untrimmedCountry = $"   {ValidAddress.Country}   ";

            var user = new UserBuilder()
                .WithAddress(new User.UserAddress
                {
                    Street = ValidAddress.Street,
                    Number = ValidAddress.Number,
                    City = ValidAddress.City,
                    PostalCode = ValidAddress.PostalCode,
                    Country = untrimmedCountry
                })
                .Build();

            user.Address.Country.ShouldBe(ValidAddress.Country); // Assert that whitespace is trimmed
        }

        #endregion

        #region Reservations and Batteries

        [Fact]
        public void HaveEmptyReservationsAndBatteriesUponCreation()
        {
            var user = new UserBuilder().Build();
            user.Reservations.ShouldBeEmpty();
            user.GuardedBatteries.ShouldBeEmpty();
        }

        #endregion
    }
}