namespace UrlShortener.DAL.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;
using ShortUrls;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = null!;

    public ICollection<ShortUrl> ShortUrls { get; set; } = null!;
}