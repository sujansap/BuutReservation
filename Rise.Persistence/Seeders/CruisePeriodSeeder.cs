using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders;

internal class CruisePeriodSeeder(ApplicationDbContext dbContext) : GeneralSeeder<CruisePeriod>(dbContext)
{
    internal static readonly IList<CruisePeriod> cruisePeriods;

    static CruisePeriodSeeder()
    {
        cruisePeriods = [
            new () { Start = DateTime.Now, End = DateTime.Now.AddDays(7) },
            new () { Start = DateTime.Now.AddDays(9), End = DateTime.Now.AddDays(25) },
            new () { Start = DateTime.Now.AddMonths(1), End = DateTime.Now.AddMonths(2) }
        ];
    }

    protected override DbSet<CruisePeriod> DbSet => _dbContext.CruisePeriods;
    protected override ICollection<CruisePeriod> Items { get => cruisePeriods; }
}
