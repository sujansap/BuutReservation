using System;
using System.Net.Http.Json;
using Rise.Shared.Users;

namespace Rise.Client.Admins;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<UserDto>> GetGuestUsers()
    {
        var result = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>("guests");
        return result ?? Enumerable.Empty<UserDto>();
    }
}