namespace UrlShortener.DAL.Queries.ShortUrls.CheckUrlExists;

public sealed class CheckUrlExistsQueryResult
{
    public bool Exists { get; set; }
    public int? ExistingId { get; set; }
}
