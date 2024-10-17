using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Reservations;
// using Rise.Domain.Reservations;

namespace Rise.Domain.Boats
{
    public interface IBoat
    {
        public string PersonalName
        {
            get;
            set;
        }

        // ! out of order admin user story 
        // public DateTime? StartOutOfOrder
        // {
        //     get;
        // }

        // public DateTime? EndOutOfOrder
        // {
        //     get;
        // }

        // ! making reservation user story 
        // public int MaximumAdults
        // {
        //     get;
        //     set;
        // }

        // public int MaximumChildren
        // {
        //     get;
        //     set;
        // }

        // public int MaximumPets
        // {
        //     get;
        //     set;
        // }

        // ! making reservation user story
        // public ICollection<IBattery> Batteries
        // {
        //     get;
        // }

        public ICollection<IReservation> Reservations
        {
            get;
        }


        // ! out of order admin user story 
        // /// <summary>
        // /// Defines the period for how long the boat is out of order.
        // /// </summary>
        // public void DefineOutOfOrderPeriod(DateTime? startOutOfOrder, DateTime? endOutOfOrder = null);

        // /// <summary>
        // /// Removes out of orders Period
        // /// </summary>
        // public void RemoveOutOfOrderPeriod();

        // /// <summary>
        // /// Checks given date is in the out of order range
        // /// </summary>
        // public bool IsInOutOfOrderPeriod(DateTime date);
    }
}