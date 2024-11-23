using Rise.Domain.TimeSlots;
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void BeAbleToAddValidTimeSlot(int amountDays)
        {
            CruisePeriod period = new CruisePeriodBuilder().Build();
            IReadOnlyList<TimeSlot> timeSlots = period.TimeSlots;
            TimeSlot timeSlot = new TimeSlotBuilder()
            .WithDate(amountDays)
            .Build();

            timeSlots.ShouldBeEmpty();
            period.AddTimeSlot(timeSlot);

            timeSlots.Count.ShouldBe(1);
            timeSlots.ShouldContain(timeSlot);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(3)]
        public void BeAbleToAddInvalidTimeSlot(int amountDays)
        {
            CruisePeriod period = new CruisePeriodBuilder().Build();
            IReadOnlyList<TimeSlot> timeSlots = period.TimeSlots;
            TimeSlot timeSlot = new TimeSlotBuilder()
            .WithDate(amountDays)
            .Build();
            Action act = () =>
                        {
                            timeSlots.ShouldBeEmpty();
                            period.AddTimeSlot(timeSlot);
                        };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}