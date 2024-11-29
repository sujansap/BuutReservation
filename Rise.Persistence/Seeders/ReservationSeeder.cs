using Microsoft.EntityFrameworkCore;
using Rise.Domain.Reservations;
using Rise.Domain.Timeslots;
using Rise.Domain.Boats;

namespace Rise.Persistence.Seeders
{
    internal class ReservationSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Reservation>(dbContext)
    {
        internal static readonly List<List<List<Reservation>>> reservations = [];

        static ReservationSeeder()
        {
            AddPastMonthLongCruisePeriodItems();
            AddWeekLongCruisePeriodItems();
            AddABitOverTwoWeekLongCruisePeriodItems();
            AddMonthLongCruisePeriodItems();
        }

        private static void AddMonthLongCruisePeriodItems()
        {
            List<List<TimeSlot>> monthLongTimeSlots = TimeSlotSeeder.timeSlots[3];

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
            List<List<TimeSlot>> pastMonthLongTimeSlots = TimeSlotSeeder.timeSlots[0];
            List<List<Reservation>> pastMonthLongReservations = [];

            foreach (List<TimeSlot> daySlots in pastMonthLongTimeSlots)
            {
                var dayReservations = new List<Reservation>
                {
                    new() {
                        Boat = BoatSeeder.boats[0],
                        TimeSlot = daySlots[0],
                        User = UserSeeder.users[0]
                    },
                    new() {
                        Boat = BoatSeeder.boats[1],
                        TimeSlot = daySlots[1],
                        User = UserSeeder.users[1]
                    },
                    new() {
                        Boat = BoatSeeder.boats[2],
                        TimeSlot = daySlots[2],
                        User = UserSeeder.users[2]
                    }
                };

                pastMonthLongReservations.Add(dayReservations);
            }

            reservations.Add(pastMonthLongReservations);
        }

        private static void AddWeekLongCruisePeriodItems()
        {
            List<List<TimeSlot>> weekLongTimeSlots = TimeSlotSeeder.timeSlots[1];

            reservations.Add([
                // Today + 0 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[0][0], Boat = BoatSeeder.boats[0] },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[0][0], Boat = BoatSeeder.boats[1] },
                    new() { User = UserSeeder.users[5], TimeSlot = weekLongTimeSlots[0][0], Boat = BoatSeeder.boats[2] },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[0][1], Boat = BoatSeeder.boats[0] },
                    new() { User = UserSeeder.users[3], TimeSlot = weekLongTimeSlots[0][1], Boat = BoatSeeder.boats[1] },
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[0][2], Boat = BoatSeeder.boats[2] },
                ],
                // Today + 1 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[1][0], Boat = BoatSeeder.boats[0] },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[1][0], Boat = BoatSeeder.boats[1] },
                    new() { User = UserSeeder.users[3], TimeSlot = weekLongTimeSlots[1][0], Boat = BoatSeeder.boats[2] },
                ],
                // Today + 2 day(s)
                [
                    new() { User = UserSeeder.users[0], TimeSlot = weekLongTimeSlots[2][0], Boat = BoatSeeder.boats[0] },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[2][1], Boat = BoatSeeder.boats[1] },
                    new() { User = UserSeeder.users[2], TimeSlot = weekLongTimeSlots[2][2], Boat = BoatSeeder.boats[2] },
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
            List<List<TimeSlot>> twoWeekLongTimeSlots = TimeSlotSeeder.timeSlots[2];

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