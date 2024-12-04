using Rise.Domain.Boats;
using Rise.Domain.TimeSlots;
using Rise.Domain.Users;

namespace Rise.Domain.Reservations
{
    public class Reservation : Entity
    {
        public readonly static int MinDaysBetweenReservation = 2;

        public int BoatId { get; set; }
        public required Boat Boat { get; set; }

        public int TimeSlotId { get; set; }
        public required TimeSlot TimeSlot { get; set; }

        public int UserId { get; set; }
        public required User User { get; set; }


        public void Cancel(bool isAdmin)
        {
            if (IsDeleted)
            {
                throw new InvalidOperationException("The reservation is already canceled.");
            }

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            if (TimeSlot.Date < currentDate)
            {
                throw new InvalidOperationException("Reservations in the past cannot be canceled.");
            }


            if (!isAdmin)
            {
                if ((TimeSlot.Date.ToDateTime(TimeOnly.MinValue) - currentDate.ToDateTime(TimeOnly.MinValue)).TotalDays < MinDaysBetweenReservation)
                {
                    throw new InvalidOperationException("Reservations can only be canceled at least 2 days before the reservation date unless canceled by an admin.");
                }
            }

            IsDeleted = true;
        }
    }
}