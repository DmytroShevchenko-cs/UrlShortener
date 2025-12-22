namespace UrlShortener.DAL.Queries.ShortUrls.GetShortUrls;

using NodaTime;

public sealed class GetShortUrlsQueryResult
{
    public List<ShortUrlDto> ShortUrls { get; set; } = new();
}

public sealed class ShortUrlDto
{
    public int Id { get; set; }
    public string ShortUrlCode { get; set; } = null!;
    public string FullUrl { get; set; } = null!;
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }
    public string CreatedAtString { get; set; } = null!;
    public string? UpdatedAtString { get; set; }
    public int? UserId { get; set; }
    public string? UserEmail { get; set; }
}