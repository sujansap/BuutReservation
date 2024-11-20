using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Users;

namespace Rise.Persistence.Seeders
{
    internal class BatterySeeder(ApplicationDbContext dbContext) : GeneralSeeder<Battery>(dbContext)
    {

        internal static readonly IList<Battery> batteries;

        static BatterySeeder()
        {
            batteries =
            [
                // Boot 1 met 3 batterijen en specifieke mentor toegewezen
                new() { Type = "Lithium-Ion", Boat = BoatSeeder.boats[0], Mentor = UserSeeder.users[0] },
                new() { Type = "Loodzuur", Boat = BoatSeeder.boats[0], Mentor = UserSeeder.users[1] },
                new() { Type = "NiMH", Boat = BoatSeeder.boats[0], Mentor = UserSeeder.users[2] },

                // Boot 2 met 3 batterijen en specifieke mentor toegewezen
                new() { Type = "Lithium-Ion", Boat = BoatSeeder.boats[1], Mentor = UserSeeder.users[3] },
                new() { Type = "Loodzuur", Boat = BoatSeeder.boats[1], Mentor = UserSeeder.users[4] },
                new() { Type = "NiMH", Boat = BoatSeeder.boats[1], Mentor = UserSeeder.users[5] },

                // Boot 3 met 3 batterijen en specifieke mentor toegewezen
                new() { Type = "Lithium-Ion", Boat = BoatSeeder.boats[2], Mentor = UserSeeder.users[6] },
                new() { Type = "Loodzuur", Boat = BoatSeeder.boats[2], Mentor = UserSeeder.users[7] },
                new() { Type = "NiMH", Boat = BoatSeeder.boats[2], Mentor = UserSeeder.users[8] }
            ];

        }
        protected override DbSet<Battery> DbSet => _dbContext.Batteries;

        protected override IEnumerable<Battery> Items => batteries;

    }
}
