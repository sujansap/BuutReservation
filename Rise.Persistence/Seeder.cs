using Microsoft.Extensions.Logging;
using Rise.Domain.Products;
using Rise.Domain.Users;

namespace Rise.Persistence;

public class Seeder
{
    private readonly ApplicationDbContext dbContext;

    public Seeder(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Seed()
    {
        if (HasAlreadyBeenSeeded())
            return;

        SeedProducts();
        SeedUsers();
    }

    private bool HasAlreadyBeenSeeded()
    {
        return dbContext.Products.Any();
    } 

    private void SeedProducts()
    {
        var products = Enumerable.Range(1, 20)
                                 .Select(i => new Product { Name = $"Product {i}"})
                                 .ToList();

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }

    private void SeedUsers() {
        var users = new List<User> {
            new() {FamilyName = "Her De Gaver"},
            new() {FamilyName = "de Clerk"},
            new() {FamilyName = "Piatti"},
            new() {FamilyName = "Chin"},
            new() {FamilyName = "Barabich"},
            new() {FamilyName = "Helks"},
         };
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }
}

