using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;

namespace Rise.Persistence.Seeders
{
    internal class BoatSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Boat>(dbContext)
    {

        internal static readonly Boat Limba = new() { PersonalName = "Limba", };
        internal static readonly Boat Leith = new() { PersonalName = "Leith", };
        internal static readonly Boat Lubeck = new() { PersonalName = "Lubeck", };

        protected override DbSet<Boat> DbSet => _dbContext.Boats;

        protected override IEnumerable<Boat> Items { get => []; }
    }
}