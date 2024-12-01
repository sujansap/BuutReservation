using System.Security.Claims;

namespace Rise.Services.Auth;

public interface IAuthContextProvider
{
    ClaimsPrincipal? User { get; }

    /// <summary>
    /// Returns the userId if user exists, null if user could not be found
    /// </summary>
    /// <returns>The userId if user exists, null if user could not be found</returns>
    int? GetUserId();
}