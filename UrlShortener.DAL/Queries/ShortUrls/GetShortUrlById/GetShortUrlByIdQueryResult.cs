using NodaTime;

namespace UrlShortener.DAL.Queries.ShortUrls.GetShortUrlById;

public sealed class GetShortUrlByIdQueryResult
{
    public int Id { get; set; }
    public string ShortUrlCode { get; set; } = null!;
    public string FullUrl { get; set; } = null!;
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }
    public int UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
}
