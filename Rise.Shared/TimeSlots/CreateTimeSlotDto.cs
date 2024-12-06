using FluentValidation;

namespace Rise.Shared.TimeSlots;

public record class CreateTimeSlotDto
{
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
    public int CruisePeriodId { get; init; }

    public class Validator : AbstractValidator<CreateTimeSlotDto>
    {
        public Validator()
        {
            RuleFor(x => x.Start)
                .NotEmpty().WithMessage("Start time is required");

            RuleFor(x => x.End)
                .NotEmpty().WithMessage("End time is required")
                .GreaterThan(x => x.Start).WithMessage("End time must be after start time");


            RuleFor(x => x.CruisePeriodId)
                .NotEmpty().WithMessage("Cruise Period ID is required")
                .GreaterThan(0).WithMessage("Cruise Period ID must be a positive number");
        }
    }
}