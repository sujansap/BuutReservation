using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.TimeSlots;
using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Boats;

public class BatteryShould
{
    [Fact]
    public void BeCreated()
    {
        Battery battery = new BatteryBuilder().Build();

        battery.Type.ShouldBe(BatteryBuilder.ValidBatteryType);
        battery.Boat.ShouldBe(BatteryBuilder.ValidBoat);
        battery.Mentor.ShouldBe(BatteryBuilder.ValidMentor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreatedWithInvalidType(string? invalidType)
    {

        Action act = () =>
        {
            Battery battery = new BatteryBuilder()
            .WithBatteryType(invalidType!)
            .Build();
        };


        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Type");
    }

    [Theory]
    [InlineData(null)]
    public void NotBeCreatedWithInvalidMentor(User? mentor)
    {

        Action act = () =>
        {
            Battery battery = new BatteryBuilder()
            .WithMentor(mentor!)
            .Build();
        };

        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Mentor");
    }

    [Fact]
    public void BeAbleToAddReservationWithValidReservation()
    {
        Battery battery = new BatteryBuilder().Build();
        battery.Reservations.ShouldBeEmpty();

        Reservation reservation = new ReservationBuilder().Build();
        battery.AddReservation(reservation);

        battery.Reservations.ShouldNotBeEmpty();
        battery.Reservations.ShouldContain(reservation);
    }

    [Fact]
    public void NotBeAbleToAddReservationWithInvalidReservation()
    {
        Battery battery = new BatteryBuilder().Build();
        battery.Reservations.ShouldBeEmpty();

        Action act = () =>
        {
            battery.AddReservation(null!);
        };

        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("reservation");
    }

    [Fact]
    public void BeAbleToRemoveReservationWithValidReservationAbsent()
    {
        Battery battery = new BatteryBuilder().Build();
        battery.Reservations.ShouldBeEmpty();

        Reservation reservation = new ReservationBuilder().Build();
        battery.AddReservation(reservation);

        Reservation notIncludedReservation = new ReservationBuilder()
        .WithBoat(
            new BoatBuilder().WithPersonalName("Other").Build()
        )
        .Build();
        battery.Reservations.ShouldNotBeEmpty();

        battery.RemoveReservation(notIncludedReservation);
        battery.Reservations.ShouldNotBeEmpty();
    }

    [Fact]
    public void BeAbleToRemoveReservationWithValidReservationPresent()
    {
        Battery battery = new BatteryBuilder().Build();
        battery.Reservations.ShouldBeEmpty();

        Reservation reservation = new ReservationBuilder().Build();
        battery.AddReservation(reservation);

        battery.Reservations.ShouldNotBeEmpty();
        battery.Reservations.ShouldContain(reservation);

        battery.RemoveReservation(reservation);
        battery.Reservations.ShouldBeEmpty();
    }

    [Fact]
    public void NotBeAbleToRemoveReservationWithInvalidReservation()
    {
        Battery battery = new BatteryBuilder().Build();
        battery.Reservations.ShouldBeEmpty();

        Reservation reservation = new ReservationBuilder().Build();
        battery.AddReservation(reservation);

        Action act = () =>
        {
            battery.RemoveReservation(null!);
        };

        act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("reservation");
    }

    private static SortedDictionary<DateOnly, List<Reservation>> CreateTestReservations(Boat boat, int timeLength = 3, int timeBuffer = 4)
    {
        CruisePeriod cruisePeriod = new CruisePeriodBuilder()
                .WithStart(DateTime.Today.AddDays(-2))
                .WithEnd(DateTime.Today.AddDays(3).AddMinutes(-1))
                .Build();

        int totalDays = (int)cruisePeriod.End.Subtract(cruisePeriod.Start).TotalDays;

        TimeOnly start = new(6, 00);
        int totalHours = 12;

        int amountOfSlots = totalHours / (timeLength + timeBuffer);

        SortedDictionary<DateOnly, List<Reservation>> reservations = [];

        for (int i = 0; i < totalDays; i++)
        {
            DateTime date = cruisePeriod.Start.AddDays(i);
            DateOnly dateOnly = DateOnly.FromDateTime(date);

            List<Reservation> datedReservations = [];

            for (int j = 0; j < amountOfSlots; j++)
            {
                TimeOnly s = start.AddHours((timeLength + timeBuffer) * j);
                TimeOnly e = s.AddHours(timeLength);

                TimeSlot timeSlot = new TimeSlotBuilder()
                .WithCruisePeriod(cruisePeriod)
                .WithDate(dateOnly)
                .WithStart(s)
                .WithEnd(e)
                .Build();

                Reservation reservation = new ReservationBuilder()
                .WithBoat(boat)
                .WithTimeSlot(timeSlot)
                .Build();

                datedReservations.Add(reservation);
            }

            reservations.Add(dateOnly, datedReservations);
        }

        return reservations;
    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(1, 5)]
    [InlineData(1, 6)]
    public void BeAvailableForTimeSlot(int timeLength, int timeBuffer)
    {
        Battery battery = new BatteryBuilder()
        .Build();

        battery.Reservations.ShouldBeEmpty();

        // Make reservations
        SortedDictionary<DateOnly, List<Reservation>> reservations = CreateTestReservations(battery.Boat, timeLength, timeBuffer);

        // Remove an reservation for place
        DateOnly date = reservations.Keys.Take(reservations.Count / 2).Last();
        List<Reservation> datedReservation = reservations[date];
        int index = datedReservation.Count / 2;
        TimeSlot timeSlot = datedReservation[index].TimeSlot;
        datedReservation.RemoveAt(index);

        foreach (var group in reservations)
        {
            group.Value.ForEach(battery.AddReservation);
        }

        battery.IsAvailableForTimeSlot(timeSlot).ShouldBeTrue();
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, 3)]
    public void NotBeAvailableForTimeSlot(int timeLength, int timeBuffer)
    {
        Battery battery = new BatteryBuilder()
        .Build();

        battery.Reservations.ShouldBeEmpty();

        // Make reservations
        SortedDictionary<DateOnly, List<Reservation>> reservations = CreateTestReservations(battery.Boat, timeLength, timeBuffer);

        // Remove an reservation for place
        DateOnly date = reservations.Keys.Take(reservations.Count / 2).Last();
        List<Reservation> datedReservation = reservations[date];
        int index = datedReservation.Count / 2;
        TimeSlot timeSlot = datedReservation[index].TimeSlot;
        datedReservation.RemoveAt(index);

        foreach (var group in reservations)
        {
            group.Value.ForEach(battery.AddReservation);
        }

        battery.IsAvailableForTimeSlot(timeSlot).ShouldBeFalse();
    }

    [Fact]
    public void BeAbleToAssignMentorAsHolder()
    {
        Battery battery = new BatteryBuilder().Build();

        battery.CurrentHolder.ShouldBeNull();
        battery.UsageCount.ShouldBe(0);
        battery.LastUsedAt.ShouldBeNull();

        battery.AssignToHolder(null, null);

        battery.CurrentHolder.ShouldBe(battery.Mentor);
        battery.UsageCount.ShouldBe(1);
        battery.LastUsedAt.ShouldNotBeNull();
        ((DateTime)battery.LastUsedAt).ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void BeAbleToAssignUserAsHolder()
    {
        Battery battery = new BatteryBuilder().Build();
        User user = new UserBuilder().Build();
        DateTime dateTime = DateTime.MaxValue;

        battery.CurrentHolder.ShouldBeNull();
        battery.UsageCount.ShouldBe(0);
        battery.LastUsedAt.ShouldBeNull();

        battery.AssignToHolder(user, dateTime);

        battery.CurrentHolder.ShouldBe(user);
        battery.UsageCount.ShouldBe(1);
        battery.LastUsedAt.ShouldBe(dateTime);
    }

    [Fact]
    public void BeSufficientRechargedWhenNeverUsed()
    {
        Battery battery = new BatteryBuilder().Build();
        DateTime currentTime = DateTime.Now;

        battery.LastUsedAt.ShouldBeNull();

        battery.HasSufficientChargingTime(currentTime).ShouldBeTrue();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(7)]
    public void BeSufficientRechargedWhenGivenTime(int timeDifference)
    {
        Battery battery = new BatteryBuilder().Build();
        DateTime currentTime = DateTime.Now;
        DateTime lastUsedAt = currentTime.AddHours(-timeDifference);

        battery.AssignToHolder(null, lastUsedAt);

        battery.HasSufficientChargingTime(currentTime).ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void NotBeSufficientRechargedWhenNotGivenTime(int timeDifference)
    {
        Battery battery = new BatteryBuilder().Build();
        DateTime currentTime = DateTime.Now;
        DateTime lastUsedAt = currentTime.AddHours(-timeDifference);

        battery.AssignToHolder(null, lastUsedAt);

        battery.HasSufficientChargingTime(currentTime).ShouldBeFalse();
    }
}




