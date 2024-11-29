using FluentValidation;
using Rise.Shared.Localization;

namespace Rise.Shared.Users;

public record class UserRegistrationModelDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PhoneNumber { get; set; }
    public required DateTime? DateOfBirth { get; set; }
    public required AddressModel Address { get; set; }

    public class AddressModel
    {
        public required string Street { get; set; }
        public required string Number { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }


    public class UserRegistrationModelDtoValidator : AbstractValidator<UserRegistrationModelDto>
    {
        public UserRegistrationModelDtoValidator(IValidatorLocalizer Localizer)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(Localizer["FirstNameRequired"])
               .MaximumLength(100).WithMessage("Your first name should not exceed 100 characters.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Please provide your last name.")
                .MaximumLength(100).WithMessage("Your last name should not exceed 100 characters.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please provide your email address.")
                .EmailAddress().WithMessage("The email address provided is not valid.")
                .MaximumLength(100).WithMessage("Your email address should not exceed 100 characters.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please provide a password.")
                .MinimumLength(8).WithMessage("Your password must be at least 8 characters long.")
                .MaximumLength(64).WithMessage("Your password should not exceed 64 characters.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please provide your phone number.")
                .Matches(@"^00[1-9][0-9]{7,14}$").WithMessage("The phone number provided is not valid.")
                .MaximumLength(25).WithMessage("Your phone number should not exceed 25 characters.");
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Please provide your date of birth.")
                .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old to register.");

            RuleFor(x => x.Address.Street).NotEmpty().WithMessage("Please provide your street address.")
                .MaximumLength(200).WithMessage("Your street address should not exceed 100 characters.");
            RuleFor(x => x.Address.Number).NotEmpty().WithMessage("Please provide your house number.")
                .MaximumLength(10).WithMessage("Your house number should not exceed 10 characters.");
            RuleFor(x => x.Address.City).NotEmpty().WithMessage("Please provide your city.")
                .MaximumLength(200).WithMessage("Your city should not exceed 200 characters.");
            RuleFor(x => x.Address.PostalCode).NotEmpty().WithMessage("Please provide your postal code.")
                .MaximumLength(100).WithMessage("Your postal code should not exceed 100 characters.");
            RuleFor(x => x.Address.Country).NotEmpty().WithMessage("Please provide your country.")
                .MaximumLength(100).WithMessage("Your country should not exceed 100 characters.");
        }

        private bool BeAtLeast18YearsOld(DateTime? dateOfBirth)
        {
            return dateOfBirth.HasValue && dateOfBirth.Value <= DateTime.Today.AddYears(-18);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserRegistrationModelDto>.CreateWithOptions((UserRegistrationModelDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
