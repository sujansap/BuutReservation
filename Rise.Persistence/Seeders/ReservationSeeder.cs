using Microsoft.EntityFrameworkCore;
using Rise.Domain.Timeslots;

namespace Rise.Persistence.Seeders
{
    internal class ReservationSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Reservation>(dbContext)
    {

        internal static readonly IList<Reservation> reservations;

        static ReservationSeeder()
        {
            reservations = [
                new (){ AmountAdults = 6, AmountChildren = 2, AmountPets = 1, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[0], },
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[1], },
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[0], },
                new (){ AmountAdults = 3, AmountChildren = 0, AmountPets = 1, Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[1], },
                new (){ AmountAdults = 2, AmountChildren = 0, AmountPets = 0, Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[0], },
                new (){ AmountAdults = 1, AmountChildren = 0, AmountPets = 0, Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[2], },

                new (){ AmountAdults = 6, AmountChildren = 2, AmountPets = 1, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[3], },
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[4], },
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[3], },
                new (){ AmountAdults = 2, AmountChildren = 0, AmountPets = 0, Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[3], },

                new (){ AmountAdults = 6, AmountChildren = 2, AmountPets = 1, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[5], },
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[6], },
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[7], },

                new (){ AmountAdults = 1, AmountChildren = 2, AmountPets = 1, Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[12], },
                new (){ AmountAdults = 1, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[13], },
                new (){ AmountAdults = 1, AmountChildren = 0, AmountPets = 1, Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[14], },
            ];
        }

        internal override DbSet<Reservation> DbSet => dbContext.Reservations;

        internal override ICollection<Reservation> Items { get => reservations; }

        internal override bool HasAlreadyBeenSeeded()
        {
            return dbContext.Reservations.Any();
        }
    }
}