using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rise.Domain.Timeslots
{
    /// <summary>
    /// The period in which it's possible to with possible/used time slots for cruising moments. 
    /// </summary>
    public interface ICruisePeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public ICollection<ITimeSlot> TimeSlots { get; }
    }
}