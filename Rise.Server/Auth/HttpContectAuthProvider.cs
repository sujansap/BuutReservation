using System;
using System.Security.Claims;
using System.Text.Json;
using Rise.Domain.Exceptions;
using Rise.Services.Auth;

namespace Rise.Server.Auth;

public class HttpContextAuthProvider(IHttpContextAccessor httpContextAccessor) : IAuthContextProvider
{
    public ClaimsPrincipal? User => httpContextAccessor!.HttpContext?.User;

    public int? GetUserId()
    {
        if (User == null){
            return null;
        }
        Claim subClaim = User.Claims.First(c => c.Type == "sub");
        Claim appMetadataClaim = User.Claims.First(c => c.Type == "app_metadata") ?? throw new UserInvalidAppMetadataException(subClaim.Value);
        Dictionary<string, string> appMetadata = JsonSerializer.Deserialize<Dictionary<string, string>>(appMetadataClaim.Value) ?? throw new UserInvalidAppMetadataException(subClaim.Value);
        if (!int.TryParse(appMetadata["userId"] ?? throw new UserInvalidAppMetadataException(subClaim.Value), out int userId))
        {
            throw new UserInvalidAppMetadataException(subClaim.Value);
        }  
        return userId;
    }

}
