using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Timeslots
{
    public class Reservation : Entity, IReservation
    {
        public int BoatId { get; set; }
        public IBoat Boat { get; set; } = default!;

        public int BatteryId { get; set; }
        public IBattery Battery { get; set; } = default!;

        public int TimeSlotId { get; set; }
        public ITimeSlot TimeSlot { get; set; } = default!;

        public ICollection<IUser> Users { get; } = [];

        private int _amountAdults;
        public int AmountAdults
        {
            get => _amountAdults;
            set => _amountAdults = Guard.Against.OutOfRange(value, "AmountAdults", 0, Guard.Against.Null(Boat, "AmountAdults", "Boat cannot be none for a reservation").MaximumAdults);
        }

        private int _amountChildren;
        public int AmountChildren
        {
            get => _amountChildren;
            set => _amountChildren = Guard.Against.OutOfRange(value, "AmountChildren", 0, Guard.Against.Null(Boat, "AmountChildren", "Boat cannot be none for a reservation").MaximumChildren);
        }

        private int _amountPets;
        public int AmountPets
        {
            get => _amountPets;
            set => _amountPets = Guard.Against.OutOfRange(value, "AmountPets", 0, Guard.Against.Null(Boat, "AmountPets", "Boat cannot be none for a reservation").MaximumPets);
        }
    }
}