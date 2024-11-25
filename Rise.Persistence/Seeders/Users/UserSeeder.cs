using Microsoft.EntityFrameworkCore;
using Rise.Domain.Users;

namespace Rise.Persistence.Seeders.Users;

/// <summary>
/// Seeder for adding users to the database
/// </summary>
/// <param name="dbContext">Database context</param>
internal class UserSeeder(ApplicationDbContext dbContext) : GeneralSeeder<User>(dbContext)
{

    /// <summary>
    /// All users
    /// </summary>
    public readonly IList<User> users = [
            new() { FamilyName = "Her De Gaver" },
        new() { FamilyName = "de Clerk" },
        new() { FamilyName = "Piatti" },
        new() { FamilyName = "Chin" },
        new() { FamilyName = "Barabich" },
        new() { FamilyName = "Helks" },
        new() { FamilyName = "Montu" },
        new() { FamilyName = "Serket" },
        new() { FamilyName = "Amunet" },
    ];

    protected override DbSet<User> DbSet => _dbContext.Users;

    protected override IEnumerable<User> Items => users;

}
