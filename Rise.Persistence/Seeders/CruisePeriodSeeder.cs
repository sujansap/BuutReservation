using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;
using Rise.Persistence.Seeders;

namespace Rise.Persistence;

internal class CruisePeriodSeeder(ApplicationDbContext dbContext) : GeneralSeeder<CruisePeriod>(dbContext)
{
    internal static readonly IList<CruisePeriod> cruisePeriods;

    static CruisePeriodSeeder()
    {
        cruisePeriods = Enumerable.Range(1, 5)
                                  .Select(i => new CruisePeriod
                                  {
                                      Start = DateTime.Now.AddDays(i),
                                      End = DateTime.Now.AddDays(i + 7)
                                  })
                                  .ToList();
    }

    internal override DbSet<CruisePeriod> DbSet => dbContext.CruisePeriods;
    internal override ICollection<CruisePeriod> Items { get => cruisePeriods; }

    internal override bool HasAlreadyBeenSeeded()
    {
        return dbContext.CruisePeriods.Any();
    }

}
