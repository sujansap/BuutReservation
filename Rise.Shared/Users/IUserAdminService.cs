using System;

namespace Rise.Shared.Users;

public interface IUserAdminService
{
    Task<IEnumerable<UserDto>> GetGuestUsers();
    Task<UserDetailDto> GetUserDetails(int userId);
    Task AddMemberRole(int userId);
    Task<int> RegisterUser(UserRegistrationModelDto userDto);
}
