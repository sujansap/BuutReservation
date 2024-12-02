
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
using Auth0.ManagementApi.Paging;
using static Rise.Shared.Users.RegisterUserDto;

namespace Rise.Services.Users;

public class UserService(ApplicationDbContext dbContext, IManagementApiClient managementApiClient, ILogger<UserService> logger) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IManagementApiClient _managementApiClient = managementApiClient;
    private readonly ILogger<UserService> _logger = logger;

    private async Task<Role> GetAuth0RoleByName(UserRole userRole)
    {
        var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = userRole.ToString() });
        return roles.FirstOrDefault() ?? throw new RoleNotFoundException($"Role '{userRole}' not found in Auth0.");
    }

    private async Task RemoveRoleFromUser(User auth0User, Role role)
    {
        await _managementApiClient.Users.RemoveRolesAsync(auth0User.UserId, new AssignRolesRequest
        {
            Roles = new[] { role.Id }
        });
    }

    private async Task AssignRoleToUser(User auth0User, Role role)
    {
        await _managementApiClient.Users.AssignRolesAsync(auth0User.UserId, new AssignRolesRequest
        {
            Roles = new[] { role.Id }
        });
    }
    public async Task<UsersPagination<UserDto>> GetUsersByRole(UserRole role, int page = 1, int pageSize = 10)
    {
        try
        {
            var auth0Role = await GetAuth0RoleByName(role);

            // Get paginated users from Auth0
            var assignedUsersPage = await GetAuth0UsersWithRetry(auth0Role.Id, page, pageSize);

            if (!assignedUsersPage.Any())
            {
                _logger.LogInformation("No users found for role {Role}", role);
                return CreateEmptyPaginationResult(page, pageSize);
            }

            var auth0Users = await Task.WhenAll(assignedUsersPage.Select(user =>
                _managementApiClient.Users.GetAsync(user.UserId)));

            var buutUserIds = auth0Users
                .Select(auth0User => auth0User.AppMetadata?["buutUserId"]?.ToString())
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (!buutUserIds.Any())
            {
                _logger.LogWarning("No valid buutUserId found in Auth0 users for role {Role}", role);
                return CreateEmptyPaginationResult(page, pageSize);
            }

            // Get users from our database matching the paginated Auth0 users
            var userDtos = await _dbContext.Users
                .Where(user => buutUserIds.Contains(user.Id.ToString()))
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FamilyName = u.FamilyName
                })
                .ToListAsync();

            return new UsersPagination<UserDto>
            {
                Items = userDtos,
                TotalCount = assignedUsersPage.Paging.Total,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (RoleNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching users by role {Role}", role);
            throw new ApplicationException($"Failed to retrieve users for role '{role}'.", ex);
        }
    }

    private async Task<IPagedList<AssignedUser>> GetAuth0UsersWithRetry(string roleId, int page, int pageSize)
    {
        const int maxRetries = 2;
        const int retryDelayInMilliseconds = 2000;

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                // Auth0 uses 0-based indexing for pages
                var assignedUsersPage = await _managementApiClient.Roles.GetUsersAsync(roleId,
                    new PaginationInfo(page - 1, pageSize, includeTotals: true));

                return assignedUsersPage;
            }
            catch (RateLimitApiException ex) when (attempt < maxRetries)
            {
                _logger.LogWarning(ex, "Rate limit exceeded while fetching users for role {RoleId}. Retrying in {RetryDelay}s...",
                    roleId, retryDelayInMilliseconds / 1000);

                await Task.Delay(retryDelayInMilliseconds);
            }
        }

        throw new ApplicationException($"Failed to fetch users for role '{roleId}' after {maxRetries + 1} attempts due to rate-limiting.");
    }

    private UsersPagination<UserDto> CreateEmptyPaginationResult(int page, int pageSize)
    {
        return new UsersPagination<UserDto>
        {
            Items = new List<UserDto>(),
            TotalCount = 0,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserDetailDto> GetUserDetails(int userId)
    {
        DomainUser user = await _dbContext.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new EntityNotFoundException(nameof(DomainUser), userId);


        return new UserDetailDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = new AddressDto
            {
                Street = user.Address.Street,
                Number = user.Address.Number,
                City = user.Address.City,
                PostalCode = user.Address.PostalCode,
                Country = user.Address.Country
            }
        };

    }
    private async Task<Auth0.ManagementApi.Models.User?> FindUserByBuutUserId(int buutUserId)
    {
        // Fetch Auth0 by buutUserId in app_metadata
        var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest() { Query = $"app_metadata.buutUserId:{buutUserId}" });
        return users.FirstOrDefault();
    }
    public async Task AddMemberRole(int userId)
    {
        var auth0User = await FindUserByBuutUserId(userId) ?? throw new EntityNotFoundException("Auth0 user", userId);

        await RemoveRoleFromUser(auth0User, await GetAuth0RoleByName(UserRole.Guest));
        await AssignRoleToUser(auth0User, await GetAuth0RoleByName(UserRole.Member));
    }

    public async Task<int> RegisterUser(RegisterUserDto userDto)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        transaction.CreateSavepoint("BeforeSavingUser");

        var user = await CreateUserInDatabase(userDto);

        await RegisterUserInAuth0(userDto, user.Id);

        transaction.Commit(); //Commit transaction after user is registered in auth0 so if it fails, user is rolled back in our db.

        return user.Id;
    }

    private async Task<DomainUser> CreateUserInDatabase(RegisterUserDto userDto)
    {
        var address = userDto.Address;
        DomainUser user = new()
        {
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            FamilyName = userDto.FamilyName,
            PhoneNumber = userDto.PhoneNumber,
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

    private async Task RegisterUserInAuth0(RegisterUserDto userDto, int userId)
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
            _logger.LogError(ex, "Rate limit exceeded.");
            await RetryRegisterUserInAuth0(userDto, userId);
        }
    }

    private async Task SendRegisterUserInAuth0Request(RegisterUserDto userDto, int userId)
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

    private async Task RetryRegisterUserInAuth0(RegisterUserDto userDto, int userId)
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
            throw new UserCreationFailedException(ErrorMessages.User.RateLimitExceeded);
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


