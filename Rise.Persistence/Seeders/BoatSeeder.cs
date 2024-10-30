using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;

namespace Rise.Persistence.Seeders
{
    internal class BoatSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Boat>(dbContext)
    {

        internal static readonly IList<Boat> boats;

        static BoatSeeder()
        {
            boats = [
                new() { PersonalName = "Limba", },
                new() { PersonalName = "Leith", },
                new() { PersonalName = "Lubeck", },
            ];
        }

        protected override DbSet<Boat> DbSet => _dbContext.Boats;

        protected override IEnumerable<Boat> Items { get => boats; }
    }
}