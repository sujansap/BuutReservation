using System;

namespace Rise.Domain.Exceptions;

public class ReservationCreationFailedException : ApplicationException
{
    public ReservationCreationFailedException(string message)
        : base("Failed to create reservation.") { }
}
