using FluentValidation;

namespace Rise.Shared.TimeSlots;

public record class CreateTimeSlotDto
{
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
    public int CruisePeriodId { get; init; }

    public class Validator : AbstractValidator<CreateTimeSlotDto>
    {
        private const double RequiredHourDuration = 1.5;
        public Validator()
        {

            RuleFor(x => x.Start)
                    .NotEmpty().WithMessage("Start time is required");

            RuleFor(x => x.End)
                    .NotEmpty().WithMessage("End time is required")
                    .GreaterThan(x => x.Start).WithMessage("End time must be after start time");

            RuleFor(x => x)
                    .Must(x => IsValidDuration(x.Start, x.End))
                    .WithMessage($"Time slot must be exactly {RequiredHourDuration} hours long");

            RuleFor(x => x.CruisePeriodId)
                    .NotEmpty().WithMessage("Cruise Period ID is required")
                    .GreaterThan(0).WithMessage("Cruise Period ID must be a positive number");
        }

        private bool IsValidDuration(TimeOnly start, TimeOnly end)
        {
            var duration = end - start;
            return Math.Abs(duration.TotalHours - RequiredHourDuration) < 0.001;
        }
    }
}