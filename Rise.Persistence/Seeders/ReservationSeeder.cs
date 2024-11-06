using Microsoft.EntityFrameworkCore;
using Rise.Domain.Reservations;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders
{
    internal class ReservationSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Reservation>(dbContext)
    {
        internal static readonly List<List<List<Reservation>>> reservations = [];

        static ReservationSeeder()
        {
            // Wait for TimeSlotSeeder to be initialized
            if (TimeSlotSeeder.timeSlots.Count == 0)
            {
                throw new InvalidOperationException("TimeSlotSeeder must be initialized before ReservationSeeder");
            }

            // Add additional check to ensure we have enough time slot groups
            if (TimeSlotSeeder.timeSlots.Count < 4) // We need at least 4 groups for all Add*Items methods
            {
                throw new InvalidOperationException($"TimeSlotSeeder must contain at least 4 groups of time slots. Current count: {TimeSlotSeeder.timeSlots.Count}");
            }

            AddMonthLongCruisePeriodItems();
            AddPastMonthLongCruisePeriodItems();
            AddWeekLongCruisePeriodItems();
            AddABitOverTwoWeekLongCruisePeriodItems();
        }

        private static void AddMonthLongCruisePeriodItems()
        {
            List<List<TimeSlot>> monthLongTimeSlots = TimeSlotSeeder.timeSlots[0];

            // Reduce the required number of days from 17 to 11
            if (monthLongTimeSlots.Count < 11)
            {
                throw new InvalidOperationException($"First time slot group must contain at least 11 days of slots. Current count: {monthLongTimeSlots.Count}");
            }

            reservations.Add([
                // Today + 1 month + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[0][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = monthLongTimeSlots[0][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = monthLongTimeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[0][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = monthLongTimeSlots[0][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[0][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 1 month + 3 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[3][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = monthLongTimeSlots[3][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[3], TimeSlot = monthLongTimeSlots[3][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[3][1], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 1 month + 5 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[5][2], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 1 month + 7 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[7][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[7][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[7][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 1 month + 10 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = monthLongTimeSlots[10][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = monthLongTimeSlots[10][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = monthLongTimeSlots[10][0], Boat = BoatSeeder.boats[2], },
                ],
            ]);
        }

        private static void AddPastMonthLongCruisePeriodItems()
        {
            List<List<TimeSlot>> pastMonthLongTimeSlots = TimeSlotSeeder.timeSlots[1];

            reservations.Add([
                // Today - 5 days or 1st november
                [
                    new() { User = UserSeeder.users[0], TimeSlot = pastMonthLongTimeSlots[0][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = pastMonthLongTimeSlots[0][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = pastMonthLongTimeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = pastMonthLongTimeSlots[0][1], Boat = BoatSeeder.boats[0], },
                ],
                // Today - 4 days or 2nd november
                [
                    new() { User = UserSeeder.users[0], TimeSlot = pastMonthLongTimeSlots[1][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = pastMonthLongTimeSlots[1][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = pastMonthLongTimeSlots[1][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = pastMonthLongTimeSlots[1][1], Boat = BoatSeeder.boats[0], },
                ],
                // Today - 3 days or 3rd november
                [
                    new() { User = UserSeeder.users[0], TimeSlot = pastMonthLongTimeSlots[2][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = pastMonthLongTimeSlots[2][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = pastMonthLongTimeSlots[2][2], Boat = BoatSeeder.boats[2], }, 
                ],
            ]);
        }

        private static void AddWeekLongCruisePeriodItems()
        {
            List<List<TimeSlot>> weekLongTimeSlots = TimeSlotSeeder.timeSlots[2];

            reservations.Add([
                // Today + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[0][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[0][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = weekLongTimeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[0][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = weekLongTimeSlots[0][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[0][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 1 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[1][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[1][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[3], TimeSlot = weekLongTimeSlots[1][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[1][1], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 2 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[2][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[2][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[2][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 5 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[4][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[4][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[4][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 6 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[5][0], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 7 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[6][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = weekLongTimeSlots[6][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[1], TimeSlot = weekLongTimeSlots[6][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[6][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[6][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[1], TimeSlot = weekLongTimeSlots[6][1], Boat = BoatSeeder.boats[2], },
                ],
            ]);
        }

        private static void AddABitOverTwoWeekLongCruisePeriodItems()
        {
            List<List<TimeSlot>> twoWeekLongTimeSlots = TimeSlotSeeder.timeSlots[3];

            reservations.Add([
                // Today + 9 day(s) + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[0][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = twoWeekLongTimeSlots[0][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[0][2], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 9 day(s) + 1 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[1][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = twoWeekLongTimeSlots[1][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[2], TimeSlot = twoWeekLongTimeSlots[1][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 2 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[2][2], Boat = BoatSeeder.boats[1], },
                ],
                // Today + 9 day(s) + 3 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[3][1], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 4 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[4][2], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = twoWeekLongTimeSlots[4][2], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = twoWeekLongTimeSlots[4][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 5 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[5][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = twoWeekLongTimeSlots[5][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[1], TimeSlot = twoWeekLongTimeSlots[5][0], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 7 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = twoWeekLongTimeSlots[7][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = twoWeekLongTimeSlots[7][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = twoWeekLongTimeSlots[7][0], Boat = BoatSeeder.boats[2], },
                ],
            ]);
        }

        protected override DbSet<Reservation> DbSet => _dbContext.Reservations;

        protected override IEnumerable<Reservation> Items { get => reservations.SelectMany(x => x.SelectMany(y => y)); }
    }
}