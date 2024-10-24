using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders
{
    internal class TimeSlotSeeder(ApplicationDbContext dbContext) : GeneralSeeder<TimeSlot>(dbContext)
    {

        internal static readonly IList<TimeSlot> timeSlots;

        static TimeSlotSeeder()
        {
            timeSlots = Enumerable.Range(0, CruisePeriodSeeder.cruisePeriods.Count)
                                  .SelectMany(i =>
                                  {
                                      CruisePeriod cruisePeriod = CruisePeriodSeeder.cruisePeriods[i];
                                      return new List<TimeSlot>{
                                        new() {
                                            Date = DateOnly.FromDateTime(cruisePeriod.Start),
                                            Start = new TimeSpan(10, 0, 0), // 10:00 AM
                                            End = new TimeSpan(13, 0, 0),   // 1:00 PM
                                            CruisePeriodId = cruisePeriod.Id
                                        },
                                        new() {
                                            Date = DateOnly.FromDateTime(cruisePeriod.Start),
                                            Start = new TimeSpan(14, 0, 0), // 2:00 PM
                                            End = new TimeSpan(17, 0, 0),   // 5:00 PM
                                            CruisePeriodId = cruisePeriod.Id
                                        },
                                        new() {
                                            Date = DateOnly.FromDateTime(cruisePeriod.Start),
                                            Start = new TimeSpan(18, 0, 0), // 6:00 PM
                                            End = new TimeSpan(21, 0, 0),   // 9:00 PM
                                            CruisePeriodId = cruisePeriod.Id
                                        }};
                                  })
                                  .ToList();

        }

        internal override DbSet<TimeSlot> DbSet => dbContext.TimeSlots;

        internal override ICollection<TimeSlot> Items { get => timeSlots; }

        internal override bool HasAlreadyBeenSeeded()
        {
            return dbContext.TimeSlots.Any();
        }
    }
}