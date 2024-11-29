using FluentValidation;

namespace Rise.Shared.Users;
public record RegisterUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string FamilyName { get; set; }
    public required string PhoneNumber { get; set; }
    public required AddressDto Address { get; set; }

    public record AddressDto
    {
        public required string Street { get; set; }
        public required string Number { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }

    public class Validator : AbstractValidator<RegisterUserDto>
    {
        public Validator()
        {
            var emailMaxLength = 69;
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(emailMaxLength).WithMessage($"Email can't be longer than {emailMaxLength} characters")
            .EmailAddress().WithMessage("Email is not a valid email address");

            var passwordMaxLength = 64;
            RuleFor(x => x.Password).NotEmpty()
            .WithMessage("Password is required")
            .MaximumLength(passwordMaxLength).WithMessage($"Password can't be longer than {passwordMaxLength} characters")
            .Matches(".*[!@#$%^&*].*")
            .WithMessage("Password requires at least one special character: !@#$%^&*")
            .Matches(".*[a-z].*")
            .WithMessage("Password requires at least one lower case letter")
            .Matches(".*[A-Z].*")
            .WithMessage("Password requires at least one upper case letter")
            .Matches(".*[0-9].*")
            .WithMessage("Password requires at least one number")
            .Matches(".{8,}")
            .WithMessage("Password requires at least 8 characters");

            var firstNameMaxLength = 100;
            RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(firstNameMaxLength).WithMessage($"First name can't be longer than {firstNameMaxLength} characters");

            var lastNameMaxLength = 100;
            RuleFor(x => x.FamilyName).NotEmpty()
            .WithMessage("Family name is required")
            .MaximumLength(lastNameMaxLength).WithMessage($"Family name can't be longer than {lastNameMaxLength} characters");

            var phoneNumberMaxLength = 100;
            RuleFor(x => x.PhoneNumber).NotEmpty()
            .WithMessage("Phone number is required")
            .MaximumLength(phoneNumberMaxLength).WithMessage($"Phone number name can't be longer than {phoneNumberMaxLength} characters")
            .Matches("^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\\s\\./0-9]*$")
            .WithMessage("Phone number is invalid");

            var streetMaxLength = 200;
            RuleFor(x => x.Address.Street).NotEmpty()
            .WithMessage("Street is required")
            .MaximumLength(streetMaxLength).WithMessage($"Street can't be longer than {streetMaxLength} characters");

            var numberMaxLength = 200;
            RuleFor(x => x.Address.Number).NotEmpty()
            .WithMessage("Number is required")
            .MaximumLength(numberMaxLength).WithMessage($"Number can't be longer than {numberMaxLength} characters");

            var cityMaxLength = 200;
            RuleFor(x => x.Address.City).NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(cityMaxLength).WithMessage($"City can't be longer than {cityMaxLength} characters");

            var postalCodeMaxLength = 100;
            RuleFor(x => x.Address.PostalCode).NotEmpty()
            .WithMessage("Postal code is required")
            .MaximumLength(postalCodeMaxLength).WithMessage($"Postal code can't be longer than {postalCodeMaxLength} characters");

            var countryMaxLength = 100;
            RuleFor(x => x.Address.Country).NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(countryMaxLength).WithMessage($"Country can't be longer than {countryMaxLength} characters");
        }
    }
}