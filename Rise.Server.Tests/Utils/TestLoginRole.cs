namespace Rise.Server.Tests.Utils
{
    public enum TestLoginRole
    {
        Guest,
        Member,
        Administrator
    }

    public static class TestLoginRoleExtensions
    {
        public static string GetEmail(this TestLoginRole role)
        {
            return role switch
            {
                TestLoginRole.Guest => "test@guest.com",
                TestLoginRole.Member => "test@member.com",
                TestLoginRole.Administrator => "test@admin.com",
                _ => throw new ArgumentException($"No email defined for role {role}")
            };
        }
        public static string GetPassword(this TestLoginRole role)
        {
            return role switch
            {
                TestLoginRole.Guest => "Test@1234",
                TestLoginRole.Member => "Test@1234",
                TestLoginRole.Administrator => "Test@1234",
                _ => throw new ArgumentException($"No password defined for role {role}")
            };
        }
        public static string GetRole(this TestLoginRole role)
        {
            return role switch
            {
                TestLoginRole.Guest => "Guest",
                TestLoginRole.Member => "Member",
                TestLoginRole.Administrator => "Administrator",
                _ => throw new ArgumentException($"No password defined for role {role}")
            };
        }

        public static string GetUserName(this TestLoginRole role)
        {
            return role switch
            {
                TestLoginRole.Guest => "Guest",
                TestLoginRole.Member => "Member",
                TestLoginRole.Administrator => "Administrator",
                _ => throw new ArgumentException($"No password defined for role {role}")
            };
        }

    }
}