using Microsoft.EntityFrameworkCore;

namespace Rise.Persistence.Seeders
{
    internal abstract class GeneralSeeder<T>(ApplicationDbContext dbContext) where T : class
    {

        internal readonly ApplicationDbContext dbContext = dbContext;
        internal abstract ICollection<T> Items { get; }
        internal abstract DbSet<T> DbSet { get; }

        public void Seed()
        {
            if (!HasAlreadyBeenSeeded())
            {
                PersistItemsToDatabase();
            }
        }

        internal abstract bool HasAlreadyBeenSeeded();

        internal void PersistItemsToDatabase()
        {
            DbSet.AddRange(Items);
            dbContext.SaveChanges();
        }

    }
}