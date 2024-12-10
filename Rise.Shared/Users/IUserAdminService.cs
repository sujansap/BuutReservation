namespace Rise.Shared.Users;

public interface IUserAdminService
{
    Task<Pagination<UserDto>> GetUsersByRole(UserRole role, int page = 1, int pageSize = 10);
    Task<Pagination<UserNameDto>> GetUsersByFullName(string? partialName, int page = 1, int pageSize = 10, CancellationToken token = default);
    Task<UserDetailDto> GetUserDetails(int userId);
    Task AddMemberRole(int userId);
    Task<int> RegisterUser(UserRegistrationModelDto userDto);
}
