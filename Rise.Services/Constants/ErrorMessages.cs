using System;

namespace Rise.Services.Constants;

public static class ErrorMessages
{
    public static class Reservation
    {
        public const string BoatAlreadyReserved = "This boat is already reserved for the selected time slot.";
        public const string UserAlreadyBooked = "You already have a booking for this time slot.";
        public const string UnexpectedError = "An unexpected error occurred while creating the reservation.";
    }
}
