using UrlShortener.DAL.Commands.ShortUrls.CreateShortUrl;
using UrlShortener.DAL.Queries.ShortUrls.CheckUrlExists;
using UrlShortener.DAL.Queries.ShortUrls.GetShortUrlById;
using UrlShortener.DAL.Queries.ShortUrls.GetShortUrlByCode;
using UrlShortener.DAL.Queries.ShortUrls.GetShortUrls;
using UrlShortener.Shared.Common.Result;

namespace UrlShortener.BLL.Services.ShortUrls;

public interface IShortUrlService
{
    Task<Result<CreateShortUrlCommandResult>> CreateShortUrlAsync(string fullUrl, int userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateShortUrlAsync(int id, string fullUrl, int userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<Result> DeleteShortUrlAsync(int id, int userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<Result<GetShortUrlsQueryResult>> GetShortUrlsAsync(int? userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<Result<GetShortUrlByIdQueryResult>> GetShortUrlByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<GetShortUrlByCodeQueryResult>> GetShortUrlByCodeAsync(string shortUrlCode, CancellationToken cancellationToken = default);
    Task<Result<CheckUrlExistsQueryResult>> CheckUrlExistsAsync(string fullUrl, int? userId = null, CancellationToken cancellationToken = default);
}

