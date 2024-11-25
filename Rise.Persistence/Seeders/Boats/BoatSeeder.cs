using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;

namespace Rise.Persistence.Seeders.Boats;
internal class BoatSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Boat>(dbContext)
{

    public readonly Boat Limba = new() { PersonalName = "Limba", };
    public readonly Boat Leith = new() { PersonalName = "Leith", };
    public readonly Boat Lubeck = new() { PersonalName = "Lubeck", };

    protected override DbSet<Boat> DbSet => _dbContext.Boats;

    protected override IEnumerable<Boat> Items => [Limba, Leith, Lubeck];
}
