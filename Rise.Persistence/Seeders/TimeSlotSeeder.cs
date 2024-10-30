using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders
{
    internal class TimeSlotSeeder(ApplicationDbContext dbContext) : GeneralSeeder<TimeSlot>(dbContext)
    {

        internal static readonly List<List<List<TimeSlot>>> timeSlots = [];

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

            timeSlots.Add([
                // Today + 0 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 2 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(9, 0, 0), End = new TimeOnly(12, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(16, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(19, 0, 0), End = new TimeOnly(22, 0, 0), },
                ],
                // Today + 3 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ],
                // Today + 5 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 6 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                ],
                // Today + 7 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ]
            ]);
        }

        private static void AddABitOverTwoWeekLongCruisePeriodItems()
        {
            CruisePeriod cruisePeriod = CruisePeriodSeeder.cruisePeriods[1];
            int cruisePeriodId = cruisePeriod.Id;
            DateOnly cruisePeriodDate = DateOnly.FromDateTime(cruisePeriod.Start);

            List<List<TimeSlot>> twoWeekLongTimeSlots = [];

            twoWeekLongTimeSlots.AddRange([
                // Today + 9 day(s) + 0 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 9 day(s) + 1 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 9 day(s) + 2 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(9, 0, 0), End = new TimeOnly(12, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(16, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(19, 0, 0), End = new TimeOnly(22, 0, 0), },

                ],
                // Today + 9 day(s) + 3 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ],
                // Today + 9 day(s) + 4 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 9 day(s) + 5 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 9 day(s) + 6 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 9 day(s) + 7 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ]
            ]);

            twoWeekLongTimeSlots.AddRange([
                // Today + 9 day(s) + 12 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },

                ],
                // Today + 9 day(s) + 13 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },

                ],
                // Today + 9 day(s) + 15 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeOnly(9, 0, 0), End = new TimeOnly(12, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(16, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeOnly(19, 0, 0), End = new TimeOnly(22, 0, 0), },
                ]
            ]);

            timeSlots.Add(twoWeekLongTimeSlots);
        }


        private static void AddMonthLongCruisePeriodItems()
        {
            CruisePeriod cruisePeriod = CruisePeriodSeeder.cruisePeriods[2];
            int cruisePeriodId = cruisePeriod.Id;
            DateOnly cruisePeriodDate = DateOnly.FromDateTime(cruisePeriod.Start);

            List<List<TimeSlot>> monthLongTimeSlots = [];

            monthLongTimeSlots.AddRange([
                // Today + 1 month + 0 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate, Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 1 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(1), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 2 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(9, 0, 0), End = new TimeOnly(12, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(16, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(2), Start = new TimeOnly(19, 0, 0), End = new TimeOnly(22, 0, 0), },
                ],
                // Today + 1 month + 3 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(3), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ],
                // Today + 1 month + 4 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(4), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 5 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(5), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },

                ],
                // Today + 1 month + 6 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(6), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 7 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(7), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ]
            ]);

            monthLongTimeSlots.AddRange([
                // Today + 1 month + 12 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(12), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 13 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(13), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 15 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeOnly(9, 0, 0), End = new TimeOnly(12, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(16, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(15), Start = new TimeOnly(19, 0, 0), End = new TimeOnly(22, 0, 0), },

                ],
                // Today + 1 month + 18 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(18), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(18), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(18), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ],
                // Today + 1 month + 19 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(19), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(19), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(19), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },

                ],
                // Today + 1 month + 20 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(20), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(20), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(20), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 25 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(25), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(25), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(25), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ],
            ]);

            monthLongTimeSlots.AddRange([
                // Today + 1 month + 27 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(27), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(13, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(27), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(17, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(27), Start = new TimeOnly(18, 0, 0), End = new TimeOnly(21, 0, 0), },
                ],
                // Today + 1 month + 29 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(29), Start = new TimeOnly(9, 0, 0), End = new TimeOnly(12, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(29), Start = new TimeOnly(14, 0, 0), End = new TimeOnly(16, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(29), Start = new TimeOnly(19, 0, 0), End = new TimeOnly(22, 0, 0), },
                ],
                // Today + 1 month + 30 day(s)
                [
                new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(30), Start = new TimeOnly(10, 0, 0), End = new TimeOnly(11, 30, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(30), Start = new TimeOnly(13, 0, 0), End = new TimeOnly(14, 0, 0), },
                    new TimeSlot() { CruisePeriodId = cruisePeriodId, Date = cruisePeriodDate.AddDays(30), Start = new TimeOnly(16, 30, 0), End = new TimeOnly(18, 45, 0), },
                ]
            ]);

            timeSlots.Add(monthLongTimeSlots);
        }

        protected override DbSet<TimeSlot> DbSet => _dbContext.TimeSlots;

        protected override IEnumerable<TimeSlot> Items { get => timeSlots.SelectMany(x => x.SelectMany(y => y)); }

    }
}