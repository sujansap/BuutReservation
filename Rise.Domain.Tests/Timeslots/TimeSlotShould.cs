using System;
using Rise.Domain.Timeslots;
using Shouldly;
using Xunit;

namespace Rise.Domain.Tests.Timeslots
{
    public class TimeSlotShould
    {
        private static readonly DateOnly ValidDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        private static readonly TimeSpan ValidStart = new TimeSpan(10, 0, 0); // 10:00 AM
        private static readonly TimeSpan ValidEnd = new TimeSpan(13, 0, 0);   // 1:00 PM

        [Fact]
        public void BeCreated()
        {
            TimeSlot timeSlot = new()
            {
                Date = ValidDate,
                Start = ValidStart,
                End = ValidEnd
            };

            timeSlot.Date.ShouldBe(ValidDate);
            timeSlot.Start.ShouldBe(ValidStart);
            timeSlot.End.ShouldBe(ValidEnd);
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeCreatedWithAnInvalidDate(string dateString)
        {
            DateOnly invalidDate = DateOnly.Parse(dateString);

            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = invalidDate, Start = ValidStart, End = ValidEnd };
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("25:00:00")]
        [InlineData("-01:00:00")]
        public void NotBeCreatedWithAnInvalidStart(string startString)
        {
            TimeSpan invalidStart = TimeSpan.Parse(startString);

            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = invalidStart, End = ValidEnd };
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("25:00:00")]
        [InlineData("-01:00:00")]
        public void NotBeCreatedWithAnInvalidEnd(string endString)
        {
            TimeSpan invalidEnd = TimeSpan.Parse(endString);

            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = ValidStart, End = invalidEnd };
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NotBeCreatedWithEndBeforeStart()
        {
            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = ValidEnd, End = ValidStart };
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeChangedToHaveAnInvalidDate(string dateString)
        {
            DateOnly invalidDate = DateOnly.Parse(dateString);

            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = ValidStart, End = ValidEnd };
                timeSlot.Date = invalidDate;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("25:00:00")]
        [InlineData("-01:00:00")]
        public void NotBeChangedToHaveAnInvalidStart(string startString)
        {
            TimeSpan invalidStart = TimeSpan.Parse(startString);

            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = ValidStart, End = ValidEnd };
                timeSlot.Start = invalidStart;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("25:00:00")]
        [InlineData("-01:00:00")]
        public void NotBeChangedToHaveAnInvalidEnd(string endString)
        {
            TimeSpan invalidEnd = TimeSpan.Parse(endString);

            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = ValidStart, End = ValidEnd };
                timeSlot.End = invalidEnd;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NotBeChangedToHaveEndBeforeStart()
        {
            Action act = () =>
            {
                TimeSlot timeSlot = new() { Date = ValidDate, Start = ValidStart, End = ValidEnd };
                timeSlot.End = ValidStart;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        // TODO make with constructor for checking relation with cruiseperiod
    }
}