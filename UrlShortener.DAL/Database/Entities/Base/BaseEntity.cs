namespace UrlShortener.DAL.Database.Entities.Base;

using NodaTime;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }
}

