using Rise.Domain.Users;

namespace Rise.Domain.Tests.TestUtilities
{
    public class UserBuilder
    {
        public const string ValidFamilyName = "Her De Gaver";

        private string familyName = ValidFamilyName;

        public UserBuilder WithFamilyName(string familyName)
        {
            this.familyName = familyName;
            return this;
        }

        public User Build()
        {
            return new()
            {
                FamilyName = familyName
            };
        }

    }
}
