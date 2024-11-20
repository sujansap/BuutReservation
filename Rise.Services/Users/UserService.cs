using Rise.Persistence;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Users;

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
}
