using System;
using FluentValidation;
using Rise.Shared.Reservations;

namespace Rise.Server.FluentValidators;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(x => x.TimeSlotId)
            .NotEmpty().WithMessage("Time Slot ID is required")
            .GreaterThan(0).WithMessage("Time Slot ID must be a positive number");

    }

}
