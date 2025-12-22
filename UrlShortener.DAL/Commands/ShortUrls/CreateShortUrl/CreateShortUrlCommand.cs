using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using UrlShortener.DAL.Database;
using UrlShortener.DAL.Database.Entities.ShortUrls;
using UrlShortener.Shared.Common.CQRS;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.DAL.Commands.ShortUrls.CreateShortUrl;

public sealed class CreateShortUrlCommand : ICommand<Result<CreateShortUrlCommandResult>>
{
    public string FullUrl { get; set; } = null!;
    public string ShortUrlCode { get; set; } = null!;
    public int UserId { get; set; }
}

public class CreateShortUrlCommandHandler(
    ILogger<CreateShortUrlCommandHandler> logger,
    BaseDbContext dbContext)
    : IRequestHandler<CreateShortUrlCommand, Result<CreateShortUrlCommandResult>>
{
    public async Task<Result<CreateShortUrlCommandResult>> Handle(
        CreateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shortUrl = new ShortUrl
            {
                FullUrl = request.FullUrl,
                ShortUrlCode = request.ShortUrlCode,
                UserId = request.UserId,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            };

            dbContext.ShortUrls.Add(shortUrl);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<CreateShortUrlCommandResult>.Success(new CreateShortUrlCommandResult
            {
                Id = shortUrl.Id,
                ShortUrlCode = shortUrl.ShortUrlCode,
                FullUrl = shortUrl.FullUrl
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while executing {Handler}", nameof(CreateShortUrlCommandHandler));
            return Result<CreateShortUrlCommandResult>.Failure($"Error while executing {nameof(CreateShortUrlCommandHandler)}");
        }
    }
}

