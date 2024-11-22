using Rise.Domain.Boats;
using Rise.Domain.Tests.TestUtilities;
using Shouldly;

namespace Rise.Domain.Tests.Boats
{
    public class BoatShould
    {
        public const string ValidPersonalNameFormatted = "Limba";

        [Theory]
        [InlineData("")]
        [InlineData("\n")]
        public void BeCreated(string extras)
        {
            Boat b = new BoatBuilder()
                .WithPersonalName(BoatBuilder.ValidPersonalName + extras)
                .Build();

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
                Boat boat = new BoatBuilder()
                .WithPersonalName(personalName!)
                .Build();
            };
            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("PersonalName");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeChangedWithInvalidPersonalName(string? personalName)
        {
            Action act = () =>
            {
                Boat boat = new BoatBuilder()
                .Build();
                boat.PersonalName = personalName!;
            };
            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("PersonalName");
        }
    }
}