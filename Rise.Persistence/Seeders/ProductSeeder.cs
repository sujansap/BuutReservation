using Microsoft.EntityFrameworkCore;
using Rise.Domain.Products;

namespace Rise.Persistence.Seeders
{
    internal class ProductSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Product>(dbContext)
    {

        internal static readonly IList<Product> products = Enumerable.Range(1, 20)
                                 .Select(i => new Product { Name = $"Product {i}" })
                                 .ToList();

        protected override DbSet<Product> DbSet => _dbContext.Products;

        protected override ICollection<Product> Items { get => products; }
    }
}