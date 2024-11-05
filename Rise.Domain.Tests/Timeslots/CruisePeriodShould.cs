using System;
using System.Collections.Generic;
using Rise.Domain.Timeslots;
using Shouldly;
using Xunit;

namespace Rise.Domain.Tests.Timeslots
{
    public class CruisePeriodShould
    {
        private static readonly DateTime ValidStart = DateTime.Today.AddDays(1);
        private static readonly DateTime ValidEnd = DateTime.Today.AddDays(2);

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-3)]
        public void BeCreated(int days)
        {
            DateTime anotherStart = ValidStart.AddDays(days);
            DateTime anotherEnd = ValidEnd.AddDays(days);
            CruisePeriod cruisePeriod = new()
            {
                Start = anotherStart,
                End = anotherEnd
            };

            cruisePeriod.Start.ShouldBe(anotherStart);
            cruisePeriod.End.ShouldBe(anotherEnd);
            cruisePeriod.TimeSlots.ShouldNotBeNull();
        }

        [Fact]
        public void NotBeCreatedWithEndBeforeStart()
        {
            Action act = () =>
            {
                CruisePeriod cruisePeriod = new() { Start = ValidEnd, End = ValidStart };
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
                CruisePeriod cruisePeriod = new() { Start = ValidStart, End = ValidEnd };
                cruisePeriod.End = invalidEnd;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NotBeChangedToHaveEndBeforeStart()
        {
            Action act = () =>
            {
                CruisePeriod cruisePeriod = new() { Start = ValidStart, End = ValidEnd };
                cruisePeriod.End = ValidStart.AddDays(-1);
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}