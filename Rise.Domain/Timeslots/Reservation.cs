// using Rise.Domain.Boats;
// using Rise.Domain.Reservations;
// using Rise.Domain.Users;

// namespace Rise.Domain.Timeslots
// {
//     public class Reservation : Entity, IReservation
//     {
//         public int BoatId { get; set; }
//         public required IBoat Boat { get; set; } = default!;

//         public int BatteryId { get; set; }
//         public required IBattery Battery { get; set; } = default!;

//         public int TimeSlotId { get; set; }
//         public required ITimeSlot TimeSlot { get; set; } = default!;

//         public ICollection<IUser> Users { get; set; } = [];

//         private int _amountAdults;
//         public int AmountAdults
//         {
//             get => _amountAdults;
//             set => _amountAdults = Guard.Against.OutOfRange(value, "AmountAdults", 0, Boat.MaximumAdults);
//         }

//         private int _amountChildren;
//         public int AmountChildren
//         {
//             get => _amountChildren;
//             set => _amountChildren = Guard.Against.OutOfRange(value, "AmountChildren", 0, Boat.MaximumChildren);
//         }

//         private int _amountPets;
//         public int AmountPets
//         {
//             get => _amountPets;
//             set => _amountPets = Guard.Against.OutOfRange(value, "AmountPets", 0, Boat.MaximumPets);
//         }
//     }
// }