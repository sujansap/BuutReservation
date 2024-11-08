using System;

namespace Rise.Domain.Exceptions;

public class UniqueConstraintViolationException : Exception
{
    public UniqueConstraintViolationException(string message)
        : base(message)
    {
    }
}
