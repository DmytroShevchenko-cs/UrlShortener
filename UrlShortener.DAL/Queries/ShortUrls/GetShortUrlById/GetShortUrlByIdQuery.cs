using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.DAL.Database;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Queries.ShortUrls.GetShortUrlById;

public sealed class GetShortUrlByIdQuery : IQuery<Result<GetShortUrlByIdQueryResult>>
{
    public int Id { get; set; }
}

public class GetShortUrlByIdQueryHandler(
    ILogger<GetShortUrlByIdQueryHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<GetShortUrlByIdQuery, Result<GetShortUrlByIdQueryResult>>
{
    public async Task<Result<GetShortUrlByIdQueryResult>> Handle(
        GetShortUrlByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shortUrl = await dbContext.ShortUrls
                .Include(s => s.User)
                .Where(s => s.Id == request.Id)
                .Select(s => new GetShortUrlByIdQueryResult
                {
                    Id = s.Id,
                    ShortUrlCode = s.ShortUrlCode,
                    FullUrl = s.FullUrl,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    UserId = s.UserId,
                    UserEmail = s.User.Email,
                    UserFirstName = s.User.FirstName,
                    UserLastName = s.User.LastName
                })
                .FirstOrDefaultAsync(cancellationToken);

            return shortUrl == null
                ? Result<GetShortUrlByIdQueryResult>.Failure("Short URL not found", 404)
                : Result<GetShortUrlByIdQueryResult>.Success(shortUrl);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(GetShortUrlByIdQueryHandler));
            return Result<GetShortUrlByIdQueryResult>.Failure($"Error while executing {nameof(GetShortUrlByIdQueryHandler)}");
        }
    }
}

