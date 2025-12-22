using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.DAL.Database;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Commands.ShortUrls.DeleteShortUrl;

public sealed class DeleteShortUrlCommand : ICommand<Result>
{
    public int Id { get; set; }
}

public class DeleteShortUrlCommandHandler(
    ILogger<DeleteShortUrlCommandHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<DeleteShortUrlCommand, Result>
{
    public async Task<Result> Handle(
        DeleteShortUrlCommand request,
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

            dbContext.ShortUrls.Remove(shortUrl);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(DeleteShortUrlCommandHandler));
            return Result.Failure($"Error while executing {nameof(DeleteShortUrlCommandHandler)}");
        }
    }
}

