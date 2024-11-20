using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Timeslots;
using Shouldly;

namespace Rise.Domain.Tests.Timeslots
{
    public class TimeSlotShould
    {

        [Fact]
        public void BeCreated()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            timeSlot.Date.ShouldBe(TimeSlotBuilder.ValidDate);
            timeSlot.Start.ShouldBe(TimeSlotBuilder.ValidStart);
            timeSlot.End.ShouldBe(TimeSlotBuilder.ValidEnd);
            timeSlot.Reservations.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeCreatedWithAnInvalidDate(string dateString)
        {
            DateOnly invalidDate = DateOnly.Parse(dateString);

            Action act = () =>
            {
                TimeSlot timeSlot = new TimeSlotBuilder().WithDate(invalidDate).Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
            .ParamName.ShouldBe("Date");
        }

        [Fact]
        public void NotBeCreatedWithEndBeforeStart()
        {
            Action act = () =>
            {
                TimeSlot timeSlot = new TimeSlotBuilder()
                .WithStart(TimeSlotBuilder.ValidEnd)
                .WithEnd(TimeSlotBuilder.ValidStart)
                .Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("End");

        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeChangedToHaveAnInvalidDate(string dateString)
        {
            DateOnly invalidDate = DateOnly.Parse(dateString);
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            Action act = () =>
            {
                timeSlot.Date = invalidDate;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("Date");

        }


        [Fact]
        public void NotBeChangedToHaveEndBeforeStart()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();
            TimeOnly invalidEnd = TimeSlotBuilder.ValidStart;

            Action act = () =>
            {
                timeSlot.End = invalidEnd;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("End");

        }

        // TODO make with constructor for checking relation with cruiseperiod
    }
}