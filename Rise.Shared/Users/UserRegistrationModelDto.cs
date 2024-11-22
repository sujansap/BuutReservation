using FluentValidation;
namespace Rise.Shared.Users;

public record class UserRegistrationModelDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public AddressModel Address { get; set; } = new();

    public class AddressModel
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}

public class UserRegistrationModelDtoValidator : AbstractValidator<UserRegistrationModelDto>
{
    public UserRegistrationModelDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?[1-9][0-9]{7,14}$").WithMessage("Invalid phone number");
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old");

        RuleFor(x => x.Address.Street).NotEmpty().WithMessage("Street is required");
        RuleFor(x => x.Address.Number).NotEmpty().WithMessage("Number is required");
        RuleFor(x => x.Address.City).NotEmpty().WithMessage("City is required");
        RuleFor(x => x.Address.PostalCode).NotEmpty().WithMessage("Postal code is required");
        RuleFor(x => x.Address.Country).NotEmpty().WithMessage("Country is required");
    }

    private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        return dateOfBirth <= DateTime.Today.AddYears(-18);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserRegistrationModelDto>.CreateWithOptions((UserRegistrationModelDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
}
