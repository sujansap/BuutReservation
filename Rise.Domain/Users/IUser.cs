using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Reservations;
// using Rise.Domain.Reservations;

namespace Rise.Domain.Users
{
    public interface IUser
    {
        public string FamilyName
        {
            get;
            set;
        }

        public ICollection<IReservation> Reservations
        {
            get;
        }

        // TODO add field + property givenName (only in authentication!)
        // TODO add field + property email (only in authentication!)
        // TODO add field + property mobilePhone (only in authentication!)
        // TODO add field + property photo (only in authentication!)
        // TODO add field + property rol (only in authentication!)
    }
}