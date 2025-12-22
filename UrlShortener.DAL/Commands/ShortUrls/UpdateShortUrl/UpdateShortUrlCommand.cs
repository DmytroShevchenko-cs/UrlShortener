using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UrlShortener.DAL.Database;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Commands.ShortUrls.UpdateShortUrl;

public sealed class UpdateShortUrlCommand : ICommand<Result>
{
    public int Id { get; set; }
    public string FullUrl { get; set; } = null!;
}

public class UpdateShortUrlCommandHandler(
    ILogger<UpdateShortUrlCommandHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<UpdateShortUrlCommand, Result>
{
    public async Task<Result> Handle(
        UpdateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shortUrl = await dbContext.ShortUrls
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shortUrl == null)
            {
                return Result.Failure("Short URL not found", 404);
            }

            shortUrl.FullUrl = request.FullUrl;
            shortUrl.UpdatedAt = SystemClock.Instance.GetCurrentInstant();

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(UpdateShortUrlCommandHandler));
            return Result.Failure($"Error while executing {nameof(UpdateShortUrlCommandHandler)}");
        }
    }
}

