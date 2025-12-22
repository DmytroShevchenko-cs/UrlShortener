using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.DAL.Commands.ShortUrls.CreateShortUrl;
using UrlShortener.DAL.Commands.ShortUrls.DeleteShortUrl;
using UrlShortener.DAL.Commands.ShortUrls.UpdateShortUrl;
using UrlShortener.DAL.Database;
using UrlShortener.DAL.Queries.ShortUrls.CheckUrlExists;
using UrlShortener.DAL.Queries.ShortUrls.GetShortUrlById;
using UrlShortener.DAL.Queries.ShortUrls.GetShortUrlByCode;
using UrlShortener.DAL.Queries.ShortUrls.GetShortUrls;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.BLL.Services.ShortUrls;

public class ShortUrlService(
    ILogger<ShortUrlService> logger,
    IMediator mediator,
    BaseDbContext dbContext)
    : IShortUrlService
{
    public async Task<Result<CreateShortUrlCommandResult>> CreateShortUrlAsync(
        string fullUrl,
        int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsValidUrl(fullUrl))
            {
                return Result<CreateShortUrlCommandResult>.Failure("Invalid URL format");
            }

            var userExists = await dbContext.Users
                .AnyAsync(u => u.Id == userId, cancellationToken);

            if (!userExists)
            {
                return Result<CreateShortUrlCommandResult>.Failure("User not found", 404);
            }

            var checkUrlResult = await CheckUrlExistsAsync(fullUrl, userId, cancellationToken);
            if (checkUrlResult.IsSuccess && checkUrlResult.Value!.Exists)
            {
                return Result<CreateShortUrlCommandResult>.Failure("This URL already exists");
            }

            var shortUrlCode = GenerateShortUrlCode();
            
            while (await dbContext.ShortUrls
                       .AnyAsync(s => s.ShortUrlCode == shortUrlCode, cancellationToken))
            {
                shortUrlCode = GenerateShortUrlCode();
            }

            var command = new CreateShortUrlCommand
            {
                FullUrl = fullUrl,
                ShortUrlCode = shortUrlCode,
                UserId = userId
            };

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Result<CreateShortUrlCommandResult>.Success(result.Value!, "Short URL created successfully!");
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while creating short URL");
            return Result<CreateShortUrlCommandResult>.Failure($"Error while creating short URL");
        }
    }

    public async Task<Result> UpdateShortUrlAsync(
        int id,
        string fullUrl,
        int userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsValidUrl(fullUrl))
            {
                return Result.Failure("Invalid URL format");
            }

            var shortUrl = await dbContext.ShortUrls
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (shortUrl == null)
            {
                return Result.Failure("Short URL not found", 404);
            }

            if (!isAdmin && shortUrl.UserId != userId)
            {
                return Result.Failure("You don't have permission to update this URL", 403);
            }

            var command = new UpdateShortUrlCommand
            {
                Id = id,
                FullUrl = fullUrl
            };

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Result.Success("Short URL updated successfully!");
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while updating short URL");
            return Result.Failure($"Error while updating short URL");
        }
    }

    public async Task<Result> DeleteShortUrlAsync(
        int id,
        int userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var shortUrl = await dbContext.ShortUrls
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (shortUrl == null)
            {
                return Result.Failure("Short URL not found", 404);
            }

            if (!isAdmin && shortUrl.UserId != userId)
            {
                return Result.Failure("You don't have permission to delete this URL", 403);
            }

            var command = new DeleteShortUrlCommand
            {
                Id = id
            };

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Result.Success("Short URL deleted successfully!");
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while deleting short URL");
            return Result.Failure($"Error while deleting short URL");
        }
    }

    public async Task<Result<GetShortUrlsQueryResult>> GetShortUrlsAsync(
        int? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetShortUrlsQuery
            {
                UserId = userId,
                IsAdmin = isAdmin
            };

            return await mediator.Send(query, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting short URLs");
            return Result<GetShortUrlsQueryResult>.Failure($"Error while getting short URLs");
        }
    }

    public string GenerateShortUrlCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<Result<GetShortUrlByIdQueryResult>> GetShortUrlByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetShortUrlByIdQuery
            {
                Id = id
            };

            return await mediator.Send(query, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting short URL by ID");
            return Result<GetShortUrlByIdQueryResult>.Failure($"Error while getting short URL by ID");
        }
    }

    public async Task<Result<GetShortUrlByCodeQueryResult>> GetShortUrlByCodeAsync(
        string shortUrlCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetShortUrlByCodeQuery
            {
                ShortUrlCode = shortUrlCode
            };

            return await mediator.Send(query, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting short URL by code");
            return Result<GetShortUrlByCodeQueryResult>.Failure($"Error while getting short URL by code");
        }
    }

    public async Task<Result<CheckUrlExistsQueryResult>> CheckUrlExistsAsync(
        string fullUrl,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new CheckUrlExistsQuery
            {
                FullUrl = fullUrl,
                UserId = userId
            };

            return await mediator.Send(query, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while checking URL existence");
            return Result<CheckUrlExistsQueryResult>.Failure($"Error while checking URL existence");
        }
    }

    public bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}

