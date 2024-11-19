using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders;

internal class CruisePeriodSeeder(ApplicationDbContext dbContext) : GeneralSeeder<CruisePeriod>(dbContext)
{
    internal static readonly IList<CruisePeriod> cruisePeriods;

    static CruisePeriodSeeder()
    {
        cruisePeriods = [
            // Past cruises
            new() { Start = DateTime.Now.AddDays(-10), End = DateTime.Now.AddDays(-7) },
            // Current/upcoming cruises
            new() { Start = DateTime.Now.AddDays(3), End = DateTime.Now.AddDays(7) },
            new() { Start = DateTime.Now.AddDays(10), End = DateTime.Now.AddDays(14) },
            new() { Start = DateTime.Now.AddMonths(1), End = DateTime.Now.AddMonths(1).AddDays(4) }
        ];
    }

    protected override DbSet<CruisePeriod> DbSet => _dbContext.CruisePeriods;
    protected override IEnumerable<CruisePeriod> Items { get => cruisePeriods; }
}
