using System;
using System.Net.Http.Json;
using Rise.Shared.Users;

namespace Rise.Client.Admins;

public class UserAdminService : IUserAdminService
{
    private readonly HttpClient _httpClient;

    public UserAdminService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<Pagination<UserDto>> GetUsersByRole(UserRole role, int page = 1, int pageSize = 10)
    {
        var result = await _httpClient.GetFromJsonAsync<Pagination<UserDto>>($"?role={role}&page={page}&pageSize={pageSize}");
        return result ?? new Pagination<UserDto>();
    }

    public async Task<UserDetailDto> GetUserDetails(int userId)
    {
        var result = await _httpClient.GetFromJsonAsync<UserDetailDto>(userId.ToString());
        return result ?? throw new Exception("Failed to get user details for user");
    }

    public async Task AddMemberRole(int userId)
    {
        await _httpClient.PostAsJsonAsync("role", new AddMemberRoleDto { UserId = userId, Role = UserRole.Member });
    }

    public Task<int> RegisterUser(UserRegistrationModelDto userDto)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>("count");
    }
}