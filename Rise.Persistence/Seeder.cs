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
        if (dbContext.Users.Any()) return;

        new UserSeeder(dbContext).Seed();
        new BoatSeeder(dbContext).Seed();
        new CruisePeriodSeeder(dbContext).Seed();
        new TimeSlotSeeder(dbContext).Seed();

        new BoatSeeder(dbContext).Seed();
        new BatterySeeder(dbContext).Seed();

        new ReservationSeeder(dbContext).Seed();
    }
}