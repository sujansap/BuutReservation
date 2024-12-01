using System;
using static Rise.Shared.Users.RegisterUserDto;

namespace Rise.Shared.Users;

public record UserDetailDto : UserDto
{
    public required string AuthId { get; set; }
    public required string FirstName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required AddressDto Address { get; set; }
}
