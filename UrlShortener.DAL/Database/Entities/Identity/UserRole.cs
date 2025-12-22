namespace UrlShortener.DAL.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class UserRole : IdentityUserRole<int>
{
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}