using Rise.Domain.Timeslots;
using Rise.Domain.Tests.TestUtilities;
using Shouldly;

namespace Rise.Domain.Tests.Timeslots
{
    public class CruisePeriodShould
    {

        [Fact]
        public void BeCreated()
        {
            CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();

            cruisePeriod.Start.ShouldBe(CruisePeriodBuilder.ValidStart);
            cruisePeriod.End.ShouldBe(CruisePeriodBuilder.ValidEnd);
            cruisePeriod.TimeSlots.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeCreatedWithAnInvalidEnd(string endString)
        {
            DateTime invalidEnd = DateTime.Parse(endString);

            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().WithEnd(invalidEnd).Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NotBeCreatedWithEndBeforeStart()
        {
            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().WithStart(CruisePeriodBuilder.ValidEnd).WithEnd(CruisePeriodBuilder.ValidStart).Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeChangedToHaveAnInvalidEnd(string endString)
        {
            DateTime invalidEnd = DateTime.Parse(endString);

            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();
                cruisePeriod.End = invalidEnd;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NotBeChangedToHaveEndBeforeStart()
        {
            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();
                cruisePeriod.End = CruisePeriodBuilder.ValidStart.AddDays(-1);
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}