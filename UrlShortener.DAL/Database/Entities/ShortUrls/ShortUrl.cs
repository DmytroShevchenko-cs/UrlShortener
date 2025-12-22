namespace UrlShortener.DAL.Database.Entities.ShortUrls;

using Base;
using Identity;

public class ShortUrl : BaseEntity
{
    public string FullUrl { get; set; } = null!;
    public string ShortUrlCode { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}