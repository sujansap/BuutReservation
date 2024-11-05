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
            AddWeekLongCruisePeriodItems();
            AddABitOverTwoWeekLongCruisePeriodItems();
            AddMonthLongCruisePeriodItems();
        }

        private static void AddWeekLongCruisePeriodItems()
        {
            List<List<TimeSlot>> timeSlots = TimeSlotSeeder.timeSlots[0];

            reservations.Add([
                // Today + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = timeSlots[0][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 1 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[1][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[1][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[3], TimeSlot = timeSlots[1][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[1][1], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 2 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[2][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[2][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[2][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 5 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[4][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[4][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[4][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 6 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[5][0], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 7 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[6][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = timeSlots[6][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[1], TimeSlot = timeSlots[6][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[6][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[6][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[1], TimeSlot = timeSlots[6][1], Boat = BoatSeeder.boats[2], },
                ],
            ]);
        }

        private static void AddABitOverTwoWeekLongCruisePeriodItems()
        {
            List<List<TimeSlot>> timeSlots = TimeSlotSeeder.timeSlots[1];

            reservations.Add([
                // Today + 9 day(s) + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[0][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][2], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 9 day(s) + 1 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[1][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[1][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[1][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 2 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[2][2], Boat = BoatSeeder.boats[1], },
                ],
                // Today + 9 day(s) + 3 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[3][1], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 4 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[4][2], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[4][2], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = timeSlots[4][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 5 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[5][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = timeSlots[5][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[1], TimeSlot = timeSlots[5][0], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 9 day(s) + 7 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[7][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[7][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = timeSlots[7][0], Boat = BoatSeeder.boats[2], },
                ],
            ]);
        }

        private static void AddMonthLongCruisePeriodItems()
        {
            List<List<TimeSlot>> timeSlots = TimeSlotSeeder.timeSlots[2];

            reservations.Add([
                // Today + 1 month + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = timeSlots[0][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][1], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[3], TimeSlot = timeSlots[0][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[0][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 1 month + 3 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[3][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[3][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[3], TimeSlot = timeSlots[3][0], Boat = BoatSeeder.boats[2], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[3][1], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 1 month + 5 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[5][2], Boat = BoatSeeder.boats[0], },
                ],
                // Today + 1 month + 20 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[13][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[13][1], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[13][2], Boat = BoatSeeder.boats[2], },
                ],
                // Today + 1 month + 29 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = timeSlots[16][0], Boat = BoatSeeder.boats[0], },
                    new() { User = UserSeeder.users[2], TimeSlot = timeSlots[16][0], Boat = BoatSeeder.boats[1], },
                    new() { User = UserSeeder.users[5], TimeSlot = timeSlots[16][0], Boat = BoatSeeder.boats[2], },
                ],
            ]);
        }

        protected override DbSet<Reservation> DbSet => _dbContext.Reservations;

        protected override IEnumerable<Reservation> Items { get => reservations.SelectMany(x => x.SelectMany(y => y)); }
    }
}