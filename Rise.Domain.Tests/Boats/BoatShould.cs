// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using NSubstitute;
// using Rise.Domain.Boats;
// using Shouldly;
// using Xunit;

// namespace Rise.Domain.Tests.Boats
// {
//     public class BoatShould
//     {
//         private const string ValidPersonalNameRaw = "Limba";
//         private const string ValidPersonalNameFormatted = "Limba";

//         private const int ValidMaximumAdults = 1;
//         private const int ValidMaximumChildren = 0;
//         private const int ValidMaximumPets = 0;

//         private static DateTime ValidMockToday = new(2024, 10, 8, 16, 30, 30);

//         [Fact]
//         public void BeCreated()
//         {
//             Boat b = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };

//             b.PersonalName.ShouldBe(ValidPersonalNameFormatted);
//             b.MaximumAdults.ShouldBe(ValidMaximumAdults);
//             b.MaximumChildren.ShouldBe(ValidMaximumChildren);
//             b.MaximumPets.ShouldBe(ValidMaximumPets);
//         }

//         [Theory]
//         [InlineData(null)]
//         [InlineData("   ")]
//         [InlineData("")]
//         public void NotBeCreatedWithInvalidPersonalName(string? personalName)
//         {
//             Action act = () =>
//             {
//                 Boat boat = new() { PersonalName = personalName!, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             };
//             act.ShouldThrow<ArgumentException>();
//         }

//         [Theory]
//         [InlineData(0)]
//         [InlineData(-1)]
//         public void NotBeCreatedWithInvalidMaximumAdults(int maximumAdults)
//         {
//             Action act = () =>
//             {
//                 Boat boat = new() { PersonalName = ValidPersonalNameRaw, MaximumAdults = maximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             };
//             act.ShouldThrow<ArgumentException>();
//         }

//         [Theory]
//         [InlineData(-1)]
//         [InlineData(-2)]
//         public void NotBeCreatedWithInvalidMaximumChildren(int maximumChildren)
//         {
//             Action act = () =>
//             {
//                 Boat boat = new() { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = maximumChildren, MaximumPets = ValidMaximumPets };
//             };
//             act.ShouldThrow<ArgumentException>();
//         }

//         [Theory]
//         [InlineData(-1)]
//         [InlineData(-2)]
//         public void NotBeCreatedWithInvalidMaximumPets(int maximumPets)
//         {
//             Action act = () =>
//             {
//                 Boat boat = new() { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = maximumPets };
//             };
//             act.ShouldThrow<ArgumentException>();
//         }

//         [Theory]
//         [InlineData(0)]
//         [InlineData(1)]
//         [InlineData(1000)]
//         [InlineData(1000 * 60)]
//         public void DefineOutOfOrderPeriodWithValidIndefinitePeriod(double startOffsetMilliseconds)
//         {
//             DateTime startOutOfOrderDate = ValidMockToday.AddMilliseconds(startOffsetMilliseconds);
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.DefineOutOfOrderPeriod(startOutOfOrderDate);
//             boat.StartOutOfOrder.ShouldBe(startOutOfOrderDate);
//             boat.EndOutOfOrder.ShouldBeNull();
//         }

//         [Theory]
//         [InlineData(-1)]
//         [InlineData(-2)]
//         [InlineData(-3)]
//         public void NotDefineOutOfOrderPeriodWithInvalidIndefinitePeriod(double startOffsetDays)
//         {
//             Action act = () =>
//             {
//                 DateTime startOutOfOrderDate = ValidMockToday.AddDays(startOffsetDays);
//                 Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//                 boat.DefineOutOfOrderPeriod(startOutOfOrderDate);
//             };
//             act.ShouldThrow<ArgumentException>();
//         }

//         [Theory]
//         [InlineData(0, 0)]
//         [InlineData(0, 1)]
//         [InlineData(0, 1000)]
//         [InlineData(0, 1000 * 60)]
//         public void DefineOutOfOrderPeriodWithValidDefinitePeriod(double startOffsetMilliseconds, double endOffsetMilliseconds)
//         {
//             DateTime startOutOfOrderDate = ValidMockToday.AddMilliseconds(startOffsetMilliseconds);
//             DateTime endOutOfOrderDate = ValidMockToday.AddMilliseconds(endOffsetMilliseconds);
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.DefineOutOfOrderPeriod(startOutOfOrderDate, endOutOfOrderDate);
//             boat.StartOutOfOrder.ShouldBe(startOutOfOrderDate);
//             boat.EndOutOfOrder.ShouldBe(endOutOfOrderDate);
//         }

//         [Theory]
//         [InlineData(0, -1)]
//         [InlineData(0, -2)]
//         [InlineData(0, -3)]
//         public void NotDefineOutOfOrderPeriodWithInvalidDefinitePeriod(double startOffsetMilliseconds, double endOffsetDays)
//         {
//             Action act = () =>
//                 {
//                     DateTime startOutOfOrderDate = ValidMockToday.AddMilliseconds(startOffsetMilliseconds);
//                     DateTime endOutOfOrderDate = ValidMockToday.AddDays(endOffsetDays);
//                     Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//                     boat.DefineOutOfOrderPeriod(startOutOfOrderDate, endOutOfOrderDate);
//                 };
//             act.ShouldThrow<ArgumentException>();
//         }

//         [Fact]
//         public void RemoveOutOfOrderPeriod()
//         {
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.RemoveOutOfOrderPeriod();
//             boat.StartOutOfOrder.ShouldBeNull();
//             boat.EndOutOfOrder.ShouldBeNull();
//         }

//         [Theory]
//         [InlineData(1)]
//         [InlineData(1000)]
//         [InlineData(1000 * 60)]
//         [InlineData(1000 * 60 * 60 * 24 * 2)]
//         public void NotBeInOutOfOrderPeriodWithActiveIndefinitePeriodAndDateOutPeriod(double offSetDate)
//         {
//             DateTime toCheckDate = ValidMockToday.AddMilliseconds(-offSetDate);
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.DefineOutOfOrderPeriod(ValidMockToday, null);
//             boat.IsInOutOfOrderPeriod(toCheckDate).ShouldBe(false);
//         }

//         [Theory]
//         [InlineData(0)]
//         [InlineData(1)]
//         [InlineData(1000)]
//         [InlineData(1000 * 60)]
//         [InlineData(1000 * 60 * 60 * 24 * 2)]
//         public void BeOutOfOrderPeriodWithActiveIndefinitePeriodAndDateInPeriod(double offSetDate)
//         {
//             DateTime toCheckDate = ValidMockToday.AddMilliseconds(offSetDate);
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.DefineOutOfOrderPeriod(ValidMockToday, null);
//             boat.IsInOutOfOrderPeriod(toCheckDate).ShouldBe(true);
//         }

//         [Theory]
//         [InlineData(1)]
//         [InlineData(1000)]
//         [InlineData(1000 * 60)]
//         [InlineData(1000 * 60 * 60 * 24 * 2)]
//         [InlineData(1000 * 60 * 60 * 24 * 4)]
//         [InlineData(1000 * 60 * 60 * 24 * 5)]
//         public void NotBeOutOfOrderPeriodWithActiveDefinitePeriodAndDateOutPeriod(double offSetDate)
//         {
//             DateTime toCheckDate = ValidMockToday.AddMilliseconds(-offSetDate);
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.DefineOutOfOrderPeriod(ValidMockToday, ValidMockToday.AddDays(3));
//             boat.IsInOutOfOrderPeriod(toCheckDate).ShouldBe(false);
//         }

//         [Theory]
//         [InlineData(0)]
//         [InlineData(1)]
//         [InlineData(1000)]
//         [InlineData(1000 * 60)]
//         [InlineData(1000 * 60 * 60 * 24 * 2)]
//         [InlineData(1000 * 60 * 60 * 24 * 3)]
//         public void BeOutOfOrderPeriodWithActiveDefinitePeriodAndDateInPeriod(double offSetDate)
//         {
//             DateTime toCheckDate = ValidMockToday.AddMilliseconds(offSetDate);
//             Boat boat = new(ValidMockToday) { PersonalName = ValidPersonalNameRaw, MaximumAdults = ValidMaximumAdults, MaximumChildren = ValidMaximumChildren, MaximumPets = ValidMaximumPets };
//             boat.DefineOutOfOrderPeriod(ValidMockToday, ValidMockToday.AddDays(3));
//             boat.IsInOutOfOrderPeriod(toCheckDate).ShouldBe(true);
//         }
//     }
// }