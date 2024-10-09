using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Reservations;

namespace Rise.Domain.Boats
{
    public interface IBattery
    {
        public string Type
        {
            get;
            set;
        }

        public double MaximumCapacity
        {
            get;
            set;
        }

        public double CurrentLoad
        {
            get;
            set;
        }

        public bool OutOfOrder
        {
            get;
            set;
        }

        public int? BoatId { get; set; }
        public IBoat? Boat { get; set; }

        public ICollection<IReservation> Reservations
        {
            get;
        }
    }
}