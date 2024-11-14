using System;
using Rise.Domain.Timeslots;
using Shouldly;
using Xunit;

namespace Rise.Domain.Tests.Timeslots
{
    public class TimeSlotShould
    {
        private static readonly DateOnly ValidDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        private static readonly TimeOnly ValidStart = new TimeOnly(10, 0, 0); // 10:00 AM
        private static readonly TimeOnly ValidEnd = new TimeOnly(13, 0, 0);   // 1:00 PM

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
            timeSlot.Reservations.ShouldBeEmpty();
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
        [InlineData(25, 0)] // Invalid hour
        [InlineData(-1, 0)] // Invalid hour
        public void NotBeCreatedWithAnInvalidStart(int hour, int minute)
        {
            // Act
            Action act = () =>
            {

                TimeSlot timeSlot = new()
                {
                    Date = ValidDate,
                    Start = new TimeOnly(hour, minute), // Using invalid TimeOnly directly
                    End = ValidEnd
                };
            };

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }


        [Theory]
        [InlineData(25, 0)] // Invalid hour
        [InlineData(-1, 0)] // Invalid hour
        public void NotBeCreatedWithAnInvalidEnd(int hour, int minute)
        {

            Action act = () =>
            {

                TimeSlot timeSlot = new()
                {
                    Date = ValidDate,
                    Start = ValidStart,
                    End = new TimeOnly(hour, minute) // Using invalid TimeOnly directly
                };
            };

            // Assert
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
        [InlineData(25, 0)] // Invalid hour
        [InlineData(-1, 0)] // Invalid hour
        public void NotBeChangedToHaveAnInvalidStart(int hour, int minute)
        {
            // Arrange
            TimeSlot timeSlot = new TimeSlot { Date = ValidDate, Start = ValidStart, End = ValidEnd };

            // Act
            Action act = () =>
            {

                TimeOnly invalidStart = new TimeOnly(hour, minute);


                timeSlot.Start = invalidStart;
            };

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }


        [Theory]
        [InlineData(25, 0)] // Invalid hour
        [InlineData(-1, 0)] // Invalid hour
        public void NotBeChangedToHaveAnInvalidEnd(int hour, int minute)
        {
            // Arrange
            TimeSlot timeSlot = new TimeSlot { Date = ValidDate, Start = ValidStart, End = ValidEnd };

            // Act
            Action act = () =>
            {
                // Create an invalid TimeOnly based on input
                TimeOnly invalidEnd = new TimeOnly(hour, minute);

                // Attempt to set the invalid end time
                timeSlot.End = invalidEnd;
            };

            // Assert
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

        [Fact]
        public void NotBeCreatedWithDateOutsideCruisePeriod()
        {
            // Arrange
            var cruisePeriod = new CruisePeriod
            {
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(5)
            };

            // Act
            Action act = () =>
            {
                TimeSlot timeSlot = new()
                {
                    CruisePeriod = cruisePeriod,
                    CruisePeriodId = 1,
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(6)), // One day after cruise period
                    Start = ValidStart,
                    End = ValidEnd
                };
            };

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("Date");
        }

        // TODO make with constructor for checking relation with cruiseperiod

    }
}