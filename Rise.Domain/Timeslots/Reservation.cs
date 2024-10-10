using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Timeslots
{
  public class Reservation : Entity, IReservation
  {
    public int BoatId { get; set; }
    public required IBoat Boat { get; set; } = default!;

    // public int BatteryId { get; set; }
    // public required IBattery Battery { get; set; } = default!;

    // public int TimeSlotId { get; set; }
    // public required ITimeSlot TimeSlot { get; set; } = default!;


//why is there no setter for users?
//       public ICollection<IUser> Users { get; } = [];
    // public ICollection<IUser> Users { get; set; } = [];

    private int _amountAdults;
    public int AmountAdults
    {
      get => _amountAdults;
      // ! making reservation user story
      // set => _amountAdults = Guard.Against.OutOfRange(value, "AmountAdults", 0, Boat.MaximumAdults);
      set => _amountAdults = value;
    }

    private int _amountChildren;
    public int AmountChildren
    {
      get => _amountChildren;
      // ! making reservation user story
      // set => _amountChildren = Guard.Against.OutOfRange(value, "AmountChildren", 0, Boat.MaximumChildren);
      set => _amountChildren = value;
    }

    private int _amountPets;
    public int AmountPets
    {
      get => _amountPets;
      // ! making reservation user story
      // set => _amountPets = Guard.Against.OutOfRange(value, "AmountPets", 0, Boat.MaximumPets);
      set => _amountPets = value;
    }
  }
}