using Microsoft.EntityFrameworkCore;
using Rise.Domain.Users;

namespace Rise.Persistence.Seeders
{
    internal class UserSeeder(ApplicationDbContext dbContext) : GeneralSeeder<User>(dbContext)
    {

        internal static readonly IList<User> users = [
            new() { FamilyName = "Her De Gaver" },
            new() { FamilyName = "de Clerk" },
            new() { FamilyName = "Piatti" },
            new() { FamilyName = "Chin" },
            new() { FamilyName = "Barabich" },
            new() { FamilyName = "Helks" },
        ];

        protected override DbSet<User> DbSet => _dbContext.Users;

        protected override ICollection<User> Items { get => users; }

    }
}