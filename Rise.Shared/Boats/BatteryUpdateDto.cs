using FluentValidation;

namespace Rise.Shared.Boats
{
    /// <summary>
    /// Info for a battery to update them
    /// </summary>
    public record BatteryUpdateDto
    {
        public required string Type { get; set; }
        public required int MentorId { get; set; }

        public class Validator : AbstractValidator<BatteryUpdateDto>
        {
            public Validator()
            {
                RuleFor(x => x.Type);

                RuleFor(x => x.MentorId)
                .NotNull()
                .GreaterThan(0)
                ;
            }
        }
    }
}


