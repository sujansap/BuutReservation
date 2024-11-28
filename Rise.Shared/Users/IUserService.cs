using System;

namespace Rise.Shared.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetGuestUsers();
    Task<UserDetailDto> GetUserDetails(int userId);
    Task AddMemberRole(int userId);
    Task<int> RegisterUser(RegisterUserDto userDto);
}
