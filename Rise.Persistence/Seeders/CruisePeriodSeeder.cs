using Rise.Domain.Timeslots;

namespace Rise.Persistence;

public class CruisePeriodSeeder
{
    private readonly ApplicationDbContext dbContext;


    public CruisePeriodSeeder(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Seed()
    {
        if (HasAlreadyBeenSeeded())
            return;

        // Seed CruisePeriods
        var cruisePeriods = Enumerable.Range(1, 5)
                                      .Select(i => new CruisePeriod
                                      {
                                          Start = DateTime.Now.AddDays(i),
                                          End = DateTime.Now.AddDays(i + 7)
                                      })
                                      .ToList();

        dbContext.CruisePeriods.AddRange(cruisePeriods);
        dbContext.SaveChanges();

        foreach (var cruisePeriod in cruisePeriods)
        {
            var timeSlots = new List<TimeSlot>
        {
            new TimeSlot
            {
                Date = DateOnly.FromDateTime(cruisePeriod.Start),
                Start = new TimeSpan(10, 0, 0), // 10:00 AM
                End = new TimeSpan(13, 0, 0),   // 1:00 PM
                CruisePeriodId = cruisePeriod.Id
            },
            new TimeSlot
            {
                Date = DateOnly.FromDateTime(cruisePeriod.Start),
                Start = new TimeSpan(14, 0, 0), // 2:00 PM
                End = new TimeSpan(17, 0, 0),   // 5:00 PM
                CruisePeriodId = cruisePeriod.Id
            },
            new TimeSlot
            {
                Date = DateOnly.FromDateTime(cruisePeriod.Start),
                Start = new TimeSpan(18, 0, 0), // 6:00 PM
                End = new TimeSpan(21, 0, 0),   // 9:00 PM
                CruisePeriodId = cruisePeriod.Id
            }
        };

            dbContext.TimeSlots.AddRange(timeSlots);
        }

        dbContext.SaveChanges();
    }

    private bool HasAlreadyBeenSeeded()
    {
        return dbContext.CruisePeriods.Any() || dbContext.TimeSlots.Any();
    }

}
