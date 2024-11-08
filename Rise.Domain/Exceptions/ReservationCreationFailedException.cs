using System;

namespace Rise.Domain.Exceptions;

public class ReservationCreationFailedException : ApplicationException
{

    private string message;
    public ReservationCreationFailedException(string message)
    {
        this.message = message;
    }
}
