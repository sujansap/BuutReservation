using Rise.Domain.Boats;
using Shouldly;

namespace Rise.Domain.Tests.Boats
{
    public class BoatShould
    {
        public const string ValidPersonalNameRaw = "Limba";
        public const string ValidPersonalNameFormatted = "Limba";

        [Fact]
        public void BeCreated()
        {
            Boat b = new() { PersonalName = ValidPersonalNameRaw };

            b.PersonalName.ShouldBe(ValidPersonalNameFormatted);
            b.Reservations.ShouldBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeCreatedWithInvalidPersonalName(string? personalName)
        {
            Action act = () =>
            {
                Boat boat = new() { PersonalName = personalName! };
            };
            act.ShouldThrow<ArgumentException>();
        }
    }
}