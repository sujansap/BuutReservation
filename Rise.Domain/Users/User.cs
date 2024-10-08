namespace Rise.Domain.Users
{
    /// <summary>
    /// User base class
    /// </summary>
    public class User: Entity
    {
        private string _familyName = default!;


        public required string FamilyName {
            get => _familyName;
            set => _familyName = Guard.Against.NullOrWhiteSpace(value);
        }
        // TODO add field + property givenName (only in authentication!)
        // TODO add field + property email (only in authentication!)
        // TODO add field + property mobilePhone (only in authentication!)
        // TODO add field + property photo (only in authentication!)
        // TODO add field + property rol (only in authentication!)
    }
}