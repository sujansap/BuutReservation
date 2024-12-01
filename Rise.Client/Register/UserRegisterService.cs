using System.Net.Http.Json;
using Rise.Shared.Users;
using Serilog;

namespace Rise.Client.Register;

public class UserRegisterService(HttpClient httpClient) : IUserService
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<IEnumerable<UserDto>> GetGuestUsers()
    {
        throw new NotImplementedException();
    }

    public Task<UserDetailDto> GetUserDetails(int userId)
    {
        throw new NotImplementedException();
    }

    public Task AddMemberRole(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> RegisterUser(UserRegistrationModelDto userDto)
    {
        Log.Information("Registering user: {userDto}", userDto);
        var result = await _httpClient.PostAsJsonAsync("register", userDto);

        if (!result.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to register user. Response: {result.ReasonPhrase}");
        }

        return await result.Content.ReadFromJsonAsync<int>();
    }
}