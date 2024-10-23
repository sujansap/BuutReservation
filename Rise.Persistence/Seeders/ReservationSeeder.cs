using Microsoft.EntityFrameworkCore;
using Rise.Domain.Reservations;

namespace Rise.Persistence.Seeders
{
    internal class ReservationSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Reservation>(dbContext)
    {

        internal static readonly IList<Reservation> reservations;

        static ReservationSeeder()
        {
            reservations = [
                new () {User = UserSeeder.users[1], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[0] },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[1] },
                new () {User = UserSeeder.users[2], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[0] },
                new () {User = UserSeeder.users[3], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[1] },
                new () {User = UserSeeder.users[5], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[0] },
                new () {User = UserSeeder.users[4], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[2] },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[3], },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[4], },
                new () {User = UserSeeder.users[2], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[3], },
                new () {User = UserSeeder.users[3], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[3], },
                new () {User = UserSeeder.users[1], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[5], },
                new () {User = UserSeeder.users[4], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[6], },
                new () {User = UserSeeder.users[5], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[7], },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[12], },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[13], },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[14], },
                new () {User = UserSeeder.users[1], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[15], },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[16], },
                new () {User = UserSeeder.users[0], Boat = BoatSeeder.boats[0], TimeSlot = TimeSlotSeeder.timeSlots[17], },
                new () {User = UserSeeder.users[2], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[15], },
                new () {User = UserSeeder.users[3], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[16], },
                new () {User = UserSeeder.users[2], Boat = BoatSeeder.boats[1], TimeSlot = TimeSlotSeeder.timeSlots[17], },
                new () {User = UserSeeder.users[5], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[15], },
                new () {User = UserSeeder.users[1], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[16], },
                new () {User = UserSeeder.users[1], Boat = BoatSeeder.boats[2], TimeSlot = TimeSlotSeeder.timeSlots[17], },
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