using Rise.Persistence;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Users;
using DomainUser = Rise.Domain.Users.User;
using Rise.Domain.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Npgsql;
using Rise.Services.Constants;
using Auth0.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Rise.Services.Users;

public class UserService(ApplicationDbContext dbContext, IManagementApiClient managementApiClient, ILogger<UserService> logger) : IUserAdminService, IUserRegisterService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IManagementApiClient _managementApiClient = managementApiClient;
    private readonly ILogger<UserService> _logger = logger;

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

    public async Task<int> RegisterUser(UserRegistrationModelDto userDto)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        transaction.CreateSavepoint("BeforeSavingUser");

        var user = await CreateUserInDatabase(userDto);

        await RegisterUserInAuth0(userDto, user.Id);

        transaction.Commit(); //Commit transaction after user is registered in auth0 so if it fails, user is rolled back in our db.

        return user.Id;
    }

    private async Task<DomainUser> CreateUserInDatabase(UserRegistrationModelDto userDto)
    {
        var address = userDto.Address;
        DomainUser user = new()
        {
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            FamilyName = userDto.FamilyName,
            PhoneNumber = userDto.PhoneNumber,
            DateOfBirth = userDto.DateOfBirth,
            Address = new()
            {
                City = address.City,
                Country = address.Country,
                Number = address.Number,
                PostalCode = address.PostalCode,
                Street = address.Street
            }
        };

        try
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error while saving user.");
            HandleDbUpdateException(ex);
            throw new UserCreationFailedException(ErrorMessages.User.UnexpectedError);
        }

        return user;
    }

    private async Task RegisterUserInAuth0(UserRegistrationModelDto userDto, int userId)
    {
        try
        {
            await SendRegisterUserInAuth0Request(userDto, userId);
        }
        catch (ErrorApiException ex)
        {
            _logger.LogError(ex, "Auth0 error");
            if (ex.Message.Contains("already exists"))
                throw new UniqueConstraintViolationException(ErrorMessages.User.EmailAlreadyExists);
            throw new UserCreationFailedException(ex.Message);
        }
        catch (RateLimitApiException ex)
        {
            _logger.LogError(ex, "Auth0 Rate limit exceeded");
            await RetryRegisterUserInAuth0(userDto, userId);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Auth0 api excpetion");
            throw new UserCreationFailedException(ex.Message);
        }
    }

    private async Task SendRegisterUserInAuth0Request(UserRegistrationModelDto userDto, int userId)
    {
        var auth0User = await _managementApiClient.Users.CreateAsync(new UserCreateRequest
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            Connection = "Username-Password-Authentication",
            Password = userDto.Password,
            AppMetadata = new Dictionary<string, object> {
                     { "buutUserId", userId },
                }
        }
           );

        var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = nameof(UserRole.Guest) });
        var role = roles.FirstOrDefault() ?? throw new RoleNotFoundException(nameof(UserRole.Guest));
        await _managementApiClient.Users.AssignRolesAsync(auth0User.UserId, new AssignRolesRequest
        {
            Roles = [role.Id]
        });
    }

    private async Task RetryRegisterUserInAuth0(UserRegistrationModelDto userDto, int userId)
    {
        var retries = 0;
        var success = false;
        while (retries <= 20 && !success)
        {
            try
            {
                await SendRegisterUserInAuth0Request(userDto, userId);
                success = true;
            }
            catch (RateLimitApiException ex)
            {
                //Delay so that auth0 api doesn't throw a rate limit exception
                _logger.LogError(ex, "Rate limit exceeded.");
                await Task.Delay(TimeSpan.FromSeconds(2));
                retries++;
            }
        }

        if (!success)
        {
            throw new UserCreationFailedException(ErrorMessages.User.Auth0RateLimitExceeded);
        }
    }

    private static void HandleDbUpdateException(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pgEx)
        {
            string message = pgEx.ConstraintName switch
            {
                DatabaseConstraints.UniqueUserEmail => ErrorMessages.User.EmailAlreadyExists,
                _ => ErrorMessages.User.UnexpectedError
            };
            throw new UniqueConstraintViolationException(message);
        }
    }
}
