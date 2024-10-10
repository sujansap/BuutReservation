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
            new() {PersonalName = "Limba", },
            new() {PersonalName = "Leith", },
            new() {PersonalName = "Lubeck",},
        ];
            //     boats = [
            //     new() {PersonalName = "Limba", MaximumAdults = 6, MaximumChildren = 2, MaximumPets = 1},
            //     new() {PersonalName = "Leith", MaximumAdults = 3, MaximumChildren = 2, MaximumPets = 1},
            //     new() {PersonalName = "Lubeck", MaximumAdults = 4, MaximumChildren = 2, MaximumPets = 1},
            // ];
            // // TODO code smell of not setting the start and end period via the constructor
            // boats[1].DefineOutOfOrderPeriod(DateTime.UtcNow);
            // boats[2].DefineOutOfOrderPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(2));
        }

        internal override DbSet<Boat> DbSet => dbContext.Boats;

        internal override ICollection<Boat> Items { get => boats; }

        internal override bool HasAlreadyBeenSeeded()
        {
            return dbContext.Boats.Any();
        }
    }
}