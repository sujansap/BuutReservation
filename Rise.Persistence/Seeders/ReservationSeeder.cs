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
                new (){ AmountAdults = 6, AmountChildren = 2, AmountPets = 1, Boat = BoatSeeder.boats[0]},
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[1]},
                new (){ AmountAdults = 2, AmountChildren = 0, AmountPets = 0, Boat = BoatSeeder.boats[2]},
                new (){ AmountAdults = 3, AmountChildren = 1, AmountPets = 0, Boat = BoatSeeder.boats[0]},
                new (){ AmountAdults = 3, AmountChildren = 0, AmountPets = 1, Boat = BoatSeeder.boats[1]},
                new (){ AmountAdults = 1, AmountChildren = 0, AmountPets = 0, Boat = BoatSeeder.boats[2]},
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