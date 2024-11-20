using NSubstitute;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Boats;

public class BatteryShould
{
    public const string ValidBatteryType = "Lithium-Ion";

    [Fact]
    public void BeCreated_WithValidType()

    {

        Boat boat = new BoatBuilder().Build();
        User mentor = new UserBuilder().Build();

        var battery = new Battery
        {
            Type = ValidBatteryType,
            Boat = boat,
            Mentor = mentor
        };


        battery.Type.ShouldBe(ValidBatteryType);
        battery.Reservations.ShouldBeEmpty();
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreated_WithInvalidType(string? invalidType)
    {

        Boat boat = new BoatBuilder().Build();
        User mentor = new UserBuilder().Build();


        Action act = () =>
        {
            var battery = new Battery
            {
                Type = invalidType!,
                Boat = boat,
                Mentor = mentor
            };
        };


        act.ShouldThrow<ArgumentException>();
    }

    [Fact]



    public void SetAndRetrieve_BoatIdAndMentorId()
    {

        Boat boat = new BoatBuilder().Build();
        User mentor = new UserBuilder().Build();

        var battery = new Battery
        {
            Type = ValidBatteryType,
            Boat = boat,
            Mentor = mentor,
            BoatId = 1,
            MentorId = 2
        };


        battery.BoatId.ShouldBe(1);
        battery.MentorId.ShouldBe(2);
    }

    [Fact]
    public void Initialize_ReservationsCollection_WhenCreated()
    {

        Boat boat = new BoatBuilder().Build();
        User mentor = new UserBuilder().Build();
        var mockReservation = Substitute.For<IReservation>();

        var battery = new Battery
        {
            Type = ValidBatteryType,
            Boat = boat,
            Mentor = mentor
        };


        battery.Reservations.Add(mockReservation);


        battery.Reservations.ShouldNotBeEmpty();
        battery.Reservations.Count.ShouldBe(1);
        battery.Reservations.ShouldContain(mockReservation);
    }
}




