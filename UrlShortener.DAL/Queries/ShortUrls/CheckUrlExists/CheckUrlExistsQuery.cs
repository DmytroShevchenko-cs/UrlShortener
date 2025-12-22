using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.DAL.Database;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Queries.ShortUrls.CheckUrlExists;

public sealed class CheckUrlExistsQuery : IQuery<Result<CheckUrlExistsQueryResult>>
{
    public string FullUrl { get; set; } = null!;
    public int? UserId { get; set; }
}

public class CheckUrlExistsQueryHandler(
    ILogger<CheckUrlExistsQueryHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<CheckUrlExistsQuery, Result<CheckUrlExistsQueryResult>>
{
    public async Task<Result<CheckUrlExistsQueryResult>> Handle(
        CheckUrlExistsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = dbContext.ShortUrls
                .Where(s => s.FullUrl == request.FullUrl);

            if (request.UserId.HasValue)
            {
                query = query.Where(s => s.UserId == request.UserId.Value);
            }

            var existingUrl = await query
                .Select(s => new { s.Id })
                .FirstOrDefaultAsync(cancellationToken);

            return Result<CheckUrlExistsQueryResult>.Success(new CheckUrlExistsQueryResult
            {
                Exists = existingUrl != null,
                ExistingId = existingUrl?.Id
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(CheckUrlExistsQueryHandler));
            return Result<CheckUrlExistsQueryResult>.Failure($"Error while executing {nameof(CheckUrlExistsQueryHandler)}");
        }
    }
}

