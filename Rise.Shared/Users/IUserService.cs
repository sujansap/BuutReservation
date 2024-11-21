using System;

namespace Rise.Shared.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetGuestUsers();
}
