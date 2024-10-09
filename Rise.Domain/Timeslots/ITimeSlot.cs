using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Domain.Timeslots
{
    /// <summary>
    /// The time slot in which a possible cruise moment can take place for a boat. 
    /// </summary>
    public interface ITimeSlot
    {

        public DateOnly Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public int CruisePeriodId { get; set; }
        public ICruisePeriod CruisePeriod { get; set; }
    }
}