using Microsoft.Extensions.Logging;
using Rise.Domain.Boats;
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
        if (!dbContext.Products.Any())
            SeedProducts();
        if (!dbContext.Users.Any())
            SeedUsers();
        if (!dbContext.Boats.Any())
            SeedBoats();
        if (!dbContext.Batteries.Any())
            SeedBatteries();
    }

    private void SeedProducts()
    {
        var products = Enumerable.Range(1, 20)
                                 .Select(i => new Product { Name = $"Product {i}" })
                                 .ToList();

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }

    private void SeedUsers()
    {
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

    private void SeedBoats()
    {
        var boats = new List<Boat> {
            new() {PersonalName = "Limba", MaximumAdults = 6, MaximumChildren = 2, MaximumPets = 1},
            new() {PersonalName = "Leith", MaximumAdults = 6, MaximumChildren = 2, MaximumPets = 1},
            new() {PersonalName = "Lubeck", MaximumAdults = 6, MaximumChildren = 2, MaximumPets = 1},
        };
        boats[1].DefineOutOfOrderPeriod(DateTime.UtcNow);
        boats[2].DefineOutOfOrderPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(2));
        dbContext.Boats.AddRange(boats);
        dbContext.SaveChanges();
    }

    private void SeedBatteries()
    {
        var batteries = new List<Battery> {
            new() {BoatId = 1, Type = "A", CurrentLoad = 75.24, MaximumCapacity = 100.0, OutOfOrder = false},
            new() {BoatId = 1, Type = "A", CurrentLoad = 69.42, MaximumCapacity = 70.0, OutOfOrder = false},
            new() {BoatId = 1, Type = "A", CurrentLoad = 24.56, MaximumCapacity = 100.0, OutOfOrder = true},
            new() {BoatId = 2, Type = "A", CurrentLoad = 44.63, MaximumCapacity = 100.0, OutOfOrder = false},
            new() {BoatId = 2, Type = "A", CurrentLoad = 4.91, MaximumCapacity = 100.0, OutOfOrder = false},
            new() {BoatId = 2, Type = "B", CurrentLoad = 12.81, MaximumCapacity = 100.0, OutOfOrder = false},
            new() {BoatId = 3, Type = "A", CurrentLoad = 58.41, MaximumCapacity = 86.0, OutOfOrder = false},
            new() {BoatId = 3, Type = "C", CurrentLoad = 32.16, MaximumCapacity = 71.0, OutOfOrder = false},
            new() {BoatId = 3, Type = "A", CurrentLoad = 33.21, MaximumCapacity = 98.0, OutOfOrder = false},
        };
        dbContext.Batteries.AddRange(batteries);
        dbContext.SaveChanges();
    }
}

