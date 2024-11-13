namespace Rise.Domain.Exceptions;

/// <summary>
/// <see cref="Exception"/> to throw when the app_metadata of the user is not what we expect.
/// </summary>
public class UserInvalidAppMetadataException : ApplicationException
{
    public UserInvalidAppMetadataException(string userId) : base($"Invalid app_metadata of user {userId}")
    {
    }
}
