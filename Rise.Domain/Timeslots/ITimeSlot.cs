using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Reservations;
// using Rise.Domain.Reservations;

namespace Rise.Domain.Timeslots
{
    /// <summary>
    /// The time slot in which a possible cruise moment can take place for a boat. 
    /// </summary>
    public interface ITimeSlot
    {

        public DateOnly Date { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }

        public int CruisePeriodId { get; set; }
        public ICruisePeriod CruisePeriod { get; set; }

        public ICollection<IReservation> Reservations
        {
            get;
        }
    }
}