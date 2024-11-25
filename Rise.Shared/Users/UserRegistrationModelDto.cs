using FluentValidation;
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
}

public class UserRegistrationModelDtoValidator : AbstractValidator<UserRegistrationModelDto>
{
    public UserRegistrationModelDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please provide your first name.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Please provide your last name.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Please provide your email address.")
            .EmailAddress().WithMessage("The email address provided is not valid.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Please provide a password.")
            .MinimumLength(6).WithMessage("Your password must be at least 6 characters long.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please provide your phone number.")
            .Matches(@"^\+?[1-9][0-9]{7,14}$").WithMessage("The phone number provided is not valid.");
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Please provide your date of birth.")
            .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old to register.");

        RuleFor(x => x.Address.Street).NotEmpty().WithMessage("Please provide your street address.");
        RuleFor(x => x.Address.Number).NotEmpty().WithMessage("Please provide your house number.");
        RuleFor(x => x.Address.City).NotEmpty().WithMessage("Please provide your city.");
        RuleFor(x => x.Address.PostalCode).NotEmpty().WithMessage("Please provide your postal code.");
        RuleFor(x => x.Address.Country).NotEmpty().WithMessage("Please provide your country.");
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
