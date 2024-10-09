using Rise.Persistence.Seeders;

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
        new ProductSeeder(dbContext).Seed();
        new UserSeeder(dbContext).Seed();

        new BoatSeeder(dbContext).Seed();
        new BatterySeeder(dbContext).Seed();

        new CruisePeriodSeeder(dbContext).Seed();
        new TimeSlotSeeder(dbContext).Seed();
    }
}