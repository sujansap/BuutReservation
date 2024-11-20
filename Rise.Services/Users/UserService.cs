using System;
using Rise.Persistence;
using Rise.Shared.Users;

namespace Rise.Services.Users;

public class UserService(ApplicationDbContext dbContext) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

}
