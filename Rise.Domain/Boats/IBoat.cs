using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Reservations;
// using Rise.Domain.Reservations;

namespace Rise.Domain.Boats
{
    public interface IBoat : IEntity
    {
        public string PersonalName
        {
            get;
            set;
        }

        public ICollection<IReservation> Reservations
        {
            get;
        }

        public ICollection<Battery> Batteries
        {
            get;
        }
    }
}