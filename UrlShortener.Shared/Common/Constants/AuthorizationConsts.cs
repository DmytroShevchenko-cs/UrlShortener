namespace UrlShortener.Shared.Common.Constants;

public class AuthorizationConsts
{
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
    
    public static class Roles
    {
        public static class Admin
        {
            public const int Id = 1;
            public const string Name = "Admin";
            public const string Description = "Admin User";
        }
        public static class User
        {
            public const int Id = 2;
            public const string Name = "User";
            public const string Description = "User";
        }
    }
}