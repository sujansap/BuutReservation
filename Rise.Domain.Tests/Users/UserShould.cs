using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Users
{
    public class UserShould
    {
        private const string ValidFamilyNameFormatted = "Her De Gaver";

        [Theory]
        [InlineData("")]
        [InlineData("\n")]
        public void BeCreated(string extras)
        {
            User u = new UserBuilder()
                .WithFamilyName(UserBuilder.ValidFamilyName + extras)
                .Build();

            u.FamilyName.ShouldBe(ValidFamilyNameFormatted);
            u.Reservations.ShouldBeEmpty();
            u.ResponsibleBatteries.ShouldBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeCreatedWithAnInvalidFamilyName(string? familyName)
        {
            Action act = () =>
            {
                User user = new UserBuilder()
                .WithFamilyName(familyName!)
                .Build();
            };
            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("FamilyName");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeChangedToHaveAnInvalidFamilyName(string? familyName)
        {
            Action act = () =>
            {
                User user = new UserBuilder()
                    .Build();
                user.FamilyName = familyName!;
            };

            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("FamilyName");

        }

        // TODO add test for givenName (only in authentication!)
        // TODO add test for email (only in authentication!)
        // TODO add test for mobilePhone (only in authentication!)
        // TODO add test for photo (only in authentication!)
        // TODO add test for rol (only in authentication!)
    }
}