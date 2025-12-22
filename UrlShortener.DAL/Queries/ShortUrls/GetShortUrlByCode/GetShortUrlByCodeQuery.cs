using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.DAL.Database;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Queries.ShortUrls.GetShortUrlByCode;

public sealed class GetShortUrlByCodeQuery : IQuery<Result<GetShortUrlByCodeQueryResult>>
{
    public string ShortUrlCode { get; set; } = null!;
}


public class GetShortUrlByCodeQueryHandler(
    ILogger<GetShortUrlByCodeQueryHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<GetShortUrlByCodeQuery, Result<GetShortUrlByCodeQueryResult>>
{
    public async Task<Result<GetShortUrlByCodeQueryResult>> Handle(
        GetShortUrlByCodeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shortUrl = await dbContext.ShortUrls
                .Where(s => s.ShortUrlCode == request.ShortUrlCode)
                .Select(s => new GetShortUrlByCodeQueryResult
                {
                    FullUrl = s.FullUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            return shortUrl == null
                ? Result<GetShortUrlByCodeQueryResult>.Failure("Short URL not found", 404)
                : Result<GetShortUrlByCodeQueryResult>.Success(shortUrl);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(GetShortUrlByCodeQueryHandler));
            return Result<GetShortUrlByCodeQueryResult>.Failure($"Error while executing {nameof(GetShortUrlByCodeQueryHandler)}");
        }
    }
}

