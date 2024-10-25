using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Users;
public class UserShould
{
    private const string ValidFamilyNameRaw = "Her De Gaver";
    private const string ValidFamilyNameFormatted = "Her De Gaver";

    [Fact]
    public void BeCreated()
    {
        User u = new() { FamilyName = ValidFamilyNameRaw };

        u.FamilyName.ShouldBe(ValidFamilyNameFormatted);
        u.Reservations.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidFamilyName(string? familyName)
    {
        Action act = () =>
        {
            User user = new() { FamilyName = familyName! };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeChangedToHaveAnInvalidFamilyName(string? familyName)
    {
        Action act = () =>
        {
            User u = new() { FamilyName = ValidFamilyNameRaw };
            u.FamilyName = familyName!;
        };

        act.ShouldThrow<ArgumentException>();
    }

    // TODO add test for givenName (only in authentication!)
    // TODO add test for email (only in authentication!)
    // TODO add test for mobilePhone (only in authentication!)
    // TODO add test for photo (only in authentication!)
    // TODO add test for rol (only in authentication!)
}
