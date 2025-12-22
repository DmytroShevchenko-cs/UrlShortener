namespace UrlShortener.DAL.Commands.ShortUrls.CreateShortUrl;

public sealed class CreateShortUrlCommandResult
{
    public int Id { get; set; }
    public string ShortUrlCode { get; set; } = null!;
    public string FullUrl { get; set; } = null!;
}