using System;

namespace Rise.Domain.Exceptions;

public class UniqueConstraintViolationException(string message) : Exception(message)
{
}
