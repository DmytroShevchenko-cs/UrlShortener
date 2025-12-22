namespace UrlShortener.DAL.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole> UserRoles { get; set; } = null!;
}