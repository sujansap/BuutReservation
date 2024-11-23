using Microsoft.EntityFrameworkCore;
using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders;

internal class CruisePeriodSeeder(ApplicationDbContext dbContext) : GeneralSeeder<CruisePeriod>(dbContext)
{
    internal static readonly IList<CruisePeriod> cruisePeriods;

    static CruisePeriodSeeder()
    {
        cruisePeriods = [
            // Past month long cruise period
            new() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now.AddDays(-2) },
            // Week long
            new() { Start = DateTime.Now, End = DateTime.Now.AddDays(7) },
            // Two week long after 2 day intermezzo
            new() { Start = DateTime.Now.AddDays(9), End = DateTime.Now.AddDays(25) },
            // Month long after a month
            new() { Start = DateTime.Now.AddMonths(1), End = DateTime.Now.AddMonths(2) }
        ];
    }

    protected override DbSet<CruisePeriod> DbSet => _dbContext.CruisePeriods;
    protected override IEnumerable<CruisePeriod> Items { get => cruisePeriods; }
}