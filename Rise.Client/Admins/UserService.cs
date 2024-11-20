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

        Console.WriteLine("Getting guest users");
        var result = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>("guests");
        Console.WriteLine("Users are here" + result);
        Console.WriteLine("Got guest users");
        return result ?? Enumerable.Empty<UserDto>();
    }
}