using System;

namespace Rise.Shared.Users;

public interface IUserService
{
    Task UpdateUserAsync(UserProfileDto userProfileDto);
}
