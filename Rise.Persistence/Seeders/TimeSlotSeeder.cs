using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders
{
    internal class TimeSlotSeeder(ApplicationDbContext dbContext) : GeneralSeeder<TimeSlot>(dbContext)
    {

        internal static readonly List<TimeSlot> timeSlots = [];

        static TimeSlotSeeder()
        {
            AddWeekLongCruisePeriodItems();
            AddABitOverTwoWeekLongCruisePeriodItems();
            AddMonthLongCruisePeriodItems();
        }

        private static void AddWeekLongCruisePeriodItems()
        {
            CruisePeriod cruisePeriod = CruisePeriodSeeder.cruisePeriods[0];
            int cruisePeriodId = cruisePeriod.Id;
            DateOnly cruisePeriodDate = DateOnly.FromDateTime(cruisePeriod.Start);

            timeSlots.AddRange([
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(9, 0, 0), End = new TimeSpan(12, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(19, 0, 0), End = new TimeSpan(22, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },
            ]);
        }

        private static void AddABitOverTwoWeekLongCruisePeriodItems()
        {
            CruisePeriod cruisePeriod = CruisePeriodSeeder.cruisePeriods[1];
            int cruisePeriodId = cruisePeriod.Id;
            DateOnly cruisePeriodDate = DateOnly.FromDateTime(cruisePeriod.Start);

            timeSlots.AddRange([
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(9, 0, 0), End = new TimeSpan(12, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(19, 0, 0), End = new TimeSpan(22, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },
            ]);

            timeSlots.AddRange([
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeSpan(9, 0, 0), End = new TimeSpan(12, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeSpan(19, 0, 0), End = new TimeSpan(22, 0, 0), },
            ]);
        }


        private static void AddMonthLongCruisePeriodItems()
        {
            CruisePeriod cruisePeriod = CruisePeriodSeeder.cruisePeriods[2];
            int cruisePeriodId = cruisePeriod.Id;
            DateOnly cruisePeriodDate = DateOnly.FromDateTime(cruisePeriod.Start);

            timeSlots.AddRange([
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(9, 0, 0), End = new TimeSpan(12, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeSpan(19, 0, 0), End = new TimeSpan(22, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },
            ]);

            timeSlots.AddRange([
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeSpan(9, 0, 0), End = new TimeSpan(12, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeSpan(19, 0, 0), End = new TimeSpan(22, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(18), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(18), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(18), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(19), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(19), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(19), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(20), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(20), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(20), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(25), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(25), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(25), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },
            ]);

            timeSlots.AddRange([
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(27), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(13, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(27), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(17, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(27), Start = new TimeSpan(18, 0, 0), End = new TimeSpan(21, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(29), Start = new TimeSpan(9, 0, 0), End = new TimeSpan(12, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(29), Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(29), Start = new TimeSpan(19, 0, 0), End = new TimeSpan(22, 0, 0), },

                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(30), Start = new TimeSpan(10, 0, 0), End = new TimeSpan(11, 30, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(30), Start = new TimeSpan(13, 0, 0), End = new TimeSpan(14, 0, 0), },
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(30), Start = new TimeSpan(16, 30, 0), End = new TimeSpan(18, 45, 0), },
            ]);
        }

        protected override DbSet<TimeSlot> DbSet => _dbContext.TimeSlots;

        protected override ICollection<TimeSlot> Items { get => timeSlots; }

    }
}