using Microsoft.EntityFrameworkCore;
using Rise.Domain.Products;

namespace Rise.Persistence.Seeders
{
    internal class ProductSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Product>(dbContext)
    {

        private static readonly IList<Product> products = Enumerable.Range(1, 20)
                                 .Select(i => new Product { Name = $"Product {i}" })
                                 .ToList();

        internal override DbSet<Product> DbSet => dbContext.Products;

        internal override ICollection<Product> Items { get => products; }

        internal override bool HasAlreadyBeenSeeded()
        {
            return dbContext.Products.Any();
        }
    }
}