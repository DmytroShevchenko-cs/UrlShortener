using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UrlShortener.DAL.Database;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Queries.ShortUrls.GetShortUrls;

public sealed class GetShortUrlsQuery : IQuery<Result<GetShortUrlsQueryResult>>
{
    public int? UserId { get; set; }
    public bool IsAdmin { get; set; }
}

public class GetShortUrlsQueryHandler(
    ILogger<GetShortUrlsQueryHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<GetShortUrlsQuery, Result<GetShortUrlsQueryResult>>
{
    public async Task<Result<GetShortUrlsQueryResult>> Handle(
        GetShortUrlsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = dbContext.ShortUrls.AsQueryable();

            if (request.IsAdmin)
            {
                var shortUrls = await query
                    .Include(s => s.User)
                    .Select(s => new ShortUrlDto
                    {
                        Id = s.Id,
                        ShortUrlCode = s.ShortUrlCode,
                        FullUrl = s.FullUrl,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt,
                        UserId = s.UserId,
                        UserEmail = s.User.Email,
                        CreatedAtString = s.CreatedAt.ToDateTimeUtc().ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdatedAtString = s.UpdatedAt.HasValue ? s.UpdatedAt.Value.ToDateTimeUtc().ToString("yyyy-MM-dd HH:mm:ss") : null
                    })
                    .ToListAsync(cancellationToken);

                return Result<GetShortUrlsQueryResult>.Success(new GetShortUrlsQueryResult
                {
                    ShortUrls = shortUrls
                });
            }

            if (request.UserId.HasValue)
            {
                var shortUrls = await query
                    .Where(s => s.UserId == request.UserId.Value)
                    .Select(s => new ShortUrlDto
                    {
                        Id = s.Id,
                        ShortUrlCode = s.ShortUrlCode,
                        FullUrl = s.FullUrl,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt,
                        UserId = s.UserId,
                        CreatedAtString = s.CreatedAt.ToDateTimeUtc().ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdatedAtString = s.UpdatedAt.HasValue ? s.UpdatedAt.Value.ToDateTimeUtc().ToString("yyyy-MM-dd HH:mm:ss") : null
                    })
                    .ToListAsync(cancellationToken);

                return Result<GetShortUrlsQueryResult>.Success(new GetShortUrlsQueryResult
                {
                    ShortUrls = shortUrls
                });
            }

            var publicShortUrls = await query
                .Select(s => new ShortUrlDto
                {
                    Id = s.Id,
                    ShortUrlCode = s.ShortUrlCode,
                    FullUrl = s.FullUrl,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    CreatedAtString = s.CreatedAt.ToDateTimeUtc().ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAtString = s.UpdatedAt.HasValue ? s.UpdatedAt.Value.ToDateTimeUtc().ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .ToListAsync(cancellationToken);

            return Result<GetShortUrlsQueryResult>.Success(new GetShortUrlsQueryResult
            {
                ShortUrls = publicShortUrls
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(GetShortUrlsQueryHandler));
            return Result<GetShortUrlsQueryResult>.Failure($"Error while executing {nameof(GetShortUrlsQueryHandler)}");
        }
    }
}

