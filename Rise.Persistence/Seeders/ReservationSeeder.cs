using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders
{
    internal class ReservationSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Reservation>(dbContext)
    {

        internal static readonly List<Reservation> reservations = [];

        static ReservationSeeder()
        {
            AddWeekLongCruisePeriodItems();
            AddABitOverTwoWeekLongCruisePeriodItems();
            AddMonthLongCruisePeriodItems();
        }

        private static void AddWeekLongCruisePeriodItems()
        {

            reservations.AddRange([
                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[0], Boat = BoatSeeder.boats[0], AmountAdults = 6, AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[0], Boat = BoatSeeder.boats[1], AmountAdults = 3, AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[0], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[1], Boat = BoatSeeder.boats[0], AmountAdults = 3, AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[1], Boat = BoatSeeder.boats[1], AmountAdults = 3, AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[2], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[3], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[3], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[3], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[4], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[5], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[6], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[2], UserSeeder.users[5], UserSeeder.users[3]]){ TimeSlot = TimeSlotSeeder.timeSlots[7], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[12], Boat = BoatSeeder.boats[0], AmountAdults = 1,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[13], Boat = BoatSeeder.boats[1], AmountAdults = 1,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[14], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 1 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[15], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[15], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[15], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },

                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[16], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[16], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[16], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[17], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[2]]){ TimeSlot = TimeSlotSeeder.timeSlots[17], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[17], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },
                ]);
        }

        private static void AddABitOverTwoWeekLongCruisePeriodItems()
        {
            reservations.AddRange([
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[20], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[21], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[21], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[21], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[22], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[23], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[24], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[2], UserSeeder.users[5], UserSeeder.users[3]]){ TimeSlot = TimeSlotSeeder.timeSlots[25], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[29], Boat = BoatSeeder.boats[0], AmountAdults = 1,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[31], Boat = BoatSeeder.boats[1], AmountAdults = 1,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[33], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 1 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[34], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[34], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[34], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },

                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[35], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[35], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[35], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[39], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[2]]){ TimeSlot = TimeSlotSeeder.timeSlots[39], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[39], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[45], Boat = BoatSeeder.boats[0], AmountAdults = 6, AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[45], Boat = BoatSeeder.boats[1], AmountAdults = 3, AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[45], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[46], Boat = BoatSeeder.boats[0], AmountAdults = 3, AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[46], Boat = BoatSeeder.boats[1], AmountAdults = 3, AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[47], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },
                ]);
        }


        private static void AddMonthLongCruisePeriodItems()
        {
            reservations.AddRange([
                            new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[52], Boat = BoatSeeder.boats[0], AmountAdults = 6, AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[52], Boat = BoatSeeder.boats[1], AmountAdults = 3, AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[52], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[53], Boat = BoatSeeder.boats[0], AmountAdults = 3, AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[53], Boat = BoatSeeder.boats[1], AmountAdults = 3, AmountChildren = 0, AmountPets = 1 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[54], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[59], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[59], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[3], UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[59], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[60], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[65], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[65], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[2], UserSeeder.users[5], UserSeeder.users[3]]){ TimeSlot = TimeSlotSeeder.timeSlots[66], Boat = BoatSeeder.boats[0], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },

                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[91], Boat = BoatSeeder.boats[0], AmountAdults = 1,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[92], Boat = BoatSeeder.boats[1], AmountAdults = 1,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[0]]){ TimeSlot = TimeSlotSeeder.timeSlots[93], Boat = BoatSeeder.boats[2], AmountAdults = 1,AmountChildren = 0, AmountPets = 1 },

                new ([UserSeeder.users[0], UserSeeder.users[1]]){ TimeSlot = TimeSlotSeeder.timeSlots[100], Boat = BoatSeeder.boats[0], AmountAdults = 6,AmountChildren = 2, AmountPets = 1 },
                new ([UserSeeder.users[2], UserSeeder.users[4]]){ TimeSlot = TimeSlotSeeder.timeSlots[100], Boat = BoatSeeder.boats[1], AmountAdults = 3,AmountChildren = 1, AmountPets = 0 },
                new ([UserSeeder.users[5]]){ TimeSlot = TimeSlotSeeder.timeSlots[100], Boat = BoatSeeder.boats[2], AmountAdults = 2,AmountChildren = 0, AmountPets = 0 },
                ]);
        }


        protected override DbSet<Reservation> DbSet => _dbContext.Reservations;

        protected override ICollection<Reservation> Items { get => reservations; }
    }
}