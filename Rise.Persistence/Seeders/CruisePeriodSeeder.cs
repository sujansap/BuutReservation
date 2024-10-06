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
      
      if(HasAlreadyBeenSeeded())
        return;

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
                    Start = DateTime.Today.AddHours(10),
                    End = DateTime.Today.AddHours(13),
                    CruisePeriodId = cruisePeriod.Id
                },
                new TimeSlot
                {
                    Start = DateTime.Today.AddHours(14),
                    End = DateTime.Today.AddHours(17),
                    CruisePeriodId = cruisePeriod.Id
                },
                new TimeSlot
                {
                    Start = DateTime.Today.AddHours(18),
                    End = DateTime.Today.AddHours(21),
                    CruisePeriodId = cruisePeriod.Id
                }
            };

            dbContext.TimeSlots.AddRange(timeSlots);
        }

        dbContext.SaveChanges();
    }

    private bool HasAlreadyBeenSeeded(){
        return dbContext.CruisePeriods.Any() || dbContext.TimeSlots.Any();
    }
  
}
