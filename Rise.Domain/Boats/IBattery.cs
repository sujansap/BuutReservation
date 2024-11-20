using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public interface IBattery : IEntity
    {
        public string Type
        {
            get;
            set;
        }

        public int BoatId { get; set; }
        public Boat Boat { get; set; }

        public int MentorId { get; set; }
        public IUser Mentor { get; set; }

        public ICollection<IReservation> Reservations
        {
            get;
        }
    }
}