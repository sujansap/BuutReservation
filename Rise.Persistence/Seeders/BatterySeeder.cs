using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;

namespace Rise.Persistence.Seeders
{
    internal class BatterySeeder(ApplicationDbContext dbContext) : GeneralSeeder<Battery>(dbContext)
    {

        internal static readonly IList<Battery> batteries;

        static BatterySeeder()
        {
            batteries = new List<Battery>
            {
                // Boot 1 met 3 batterijen en specifieke mentor toegewezen
                new Battery { Type = "Lithium-Ion", BoatId = 1, MentorId = 1 },
                new Battery { Type = "Loodzuur", BoatId = 1, MentorId = 2 },
                new Battery { Type = "NiMH", BoatId = 1, MentorId = 3 },

                // Boot 2 met 3 batterijen en specifieke mentor toegewezen
                new Battery { Type = "Lithium-Ion", BoatId = 2, MentorId = 4 },
                new Battery { Type = "Loodzuur", BoatId = 2, MentorId = 5 },
                new Battery { Type = "NiMH", BoatId = 2, MentorId = 6 },

                // Boot 3 met 3 batterijen en specifieke mentor toegewezen
                new Battery { Type = "Lithium-Ion", BoatId = 3, MentorId = 7 },
                new Battery { Type = "Loodzuur", BoatId = 3, MentorId = 8 },
                new Battery { Type = "NiMH", BoatId = 3, MentorId = 9 }
            };

        }
        protected override DbSet<Battery> DbSet => _dbContext.Batteries;

        protected override IEnumerable<Battery> Items => batteries;

    }
}
