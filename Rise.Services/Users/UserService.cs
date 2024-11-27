using Rise.Persistence;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Users;
using DomainUser = Rise.Domain.Users.User;
using Rise.Domain.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;

namespace Rise.Services.Users;

public class UserService(ApplicationDbContext dbContext, IManagementApiClient managementApiClient) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IManagementApiClient _managementApiClient = managementApiClient;

    public async Task<IEnumerable<UserDto>> GetGuestUsers()
    {
        return await _dbContext.Users
        .Select(user => new UserDto()
        {
            Id = user.Id,
            FamilyName = user.FamilyName
        }).ToListAsync();

    }

    public async Task<UserDetailDto> GetUserDetails(int userId)
    {

        /*IS THIS IT?*/
        DomainUser user = await _dbContext.Users.FindAsync(userId) ?? throw new EntityNotFoundException(nameof(DomainUser), userId);

        return new UserDetailDto()
        {
            Id = user.Id,
            FamilyName = user.FamilyName
        };

    }

    public async Task AddMemberRole(int userId)
    {
       DomainUser user = await _dbContext.Users.FindAsync(userId) ?? throw new EntityNotFoundException(nameof(DomainUser), userId);
        /**TODO: Implement adding role to user using management api (AUTH0)**/
    }

    public async Task RegisterUser(RegisterUserDto userDto)
    {
        
        await _managementApiClient.Users.CreateAsync(new UserCreateRequest
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            Connection = "Username-Password-Authentication",
            Password = userDto.Password,
            AppMetadata = new Dictionary<string, object> { }
        }

        );
    }
}
