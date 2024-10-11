// using Microsoft.EntityFrameworkCore;
// using Rise.Domain.Boats;

// namespace Rise.Persistence.Seeders
// {
//     internal class BatterySeeder(ApplicationDbContext dbContext) : GeneralSeeder<Battery>(dbContext)
//     {

//         internal static readonly IList<Battery> batteries;

//         static BatterySeeder()
//         {
//             batteries = [
//             new() {Boat = BoatSeeder.boats[0], Type = "A", CurrentLoad = 75.24, MaximumCapacity = 100.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[0], Type = "A", CurrentLoad = 69.42, MaximumCapacity = 70.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[0], Type = "A", CurrentLoad = 24.56, MaximumCapacity = 100.0, OutOfOrder = true},
//             new() {Boat = BoatSeeder.boats[1], Type = "A", CurrentLoad = 44.63, MaximumCapacity = 100.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[1], Type = "A", CurrentLoad = 4.91, MaximumCapacity = 100.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[1], Type = "A", CurrentLoad = 12.81, MaximumCapacity = 100.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[2], Type = "A", CurrentLoad = 58.41, MaximumCapacity = 86.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[2], Type = "A", CurrentLoad = 32.16, MaximumCapacity = 71.0, OutOfOrder = false},
//             new() {Boat = BoatSeeder.boats[2], Type = "A", CurrentLoad = 33.21, MaximumCapacity = 98.0, OutOfOrder = false},
//             new() {Boat = null, Type = "B", CurrentLoad = 100.00, MaximumCapacity = 100.00, OutOfOrder = false},
//         ];
//         }

//         internal override DbSet<Battery> DbSet => dbContext.Batteries;

//         internal override ICollection<Battery> Items { get => batteries; }

//         internal override bool HasAlreadyBeenSeeded()
//         {
//             return dbContext.Batteries.Any();
//         }
//     }
// }