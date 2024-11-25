using Rise.Persistence;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Users;
using Rise.Domain.Users;

namespace Rise.Services.Users;

public class UserService(ApplicationDbContext dbContext) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

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
        User user = await _dbContext.Users.FindAsync(userId) ?? throw new Domain.Exceptions.EntityNotFoundException(nameof(User), userId);

        return new UserDetailDto()
        {
            Id = user.Id,
            FamilyName = user.FamilyName
        };

    }

    public async Task AddMemberRole(int userId)
    {
        User user = await _dbContext.Users.FindAsync(userId) ?? throw new EntityNotFoundException(nameof(User), userId);
        /**TODO: Implement adding role to user using management api (AUTH0)**/
    }
}
