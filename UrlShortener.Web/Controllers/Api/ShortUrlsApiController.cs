using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.BLL.Services.ShortUrls;
using UrlShortener.DAL.Database.Entities.Identity;
using UrlShortener.Shared.Common.Constants;

namespace UrlShortener.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlsApiController : ControllerBase
{
    private readonly IShortUrlService _shortUrlService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ShortUrlsApiController> _logger;

    public ShortUrlsApiController(
        IShortUrlService shortUrlService,
        UserManager<User> userManager,
        ILogger<ShortUrlsApiController> logger)
    {
        _shortUrlService = shortUrlService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetShortUrls(CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync();
        var isAdmin = User.IsInRole(AuthorizationConsts.Roles.Admin.Name);

        var result = await _shortUrlService.GetShortUrlsAsync(userId, isAdmin, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { error = result.Error ?? result.Message ?? "Failed to load URLs" });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetShortUrlById(int id, CancellationToken cancellationToken)
    {
        var result = await _shortUrlService.GetShortUrlByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(new { error = result.Error ?? result.Message ?? "Short URL not found" });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortUrlRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullUrl))
        {
            return BadRequest(new { error = "Full URL is required" });
        }

        var userId = await GetCurrentUserIdAsync();
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var result = await _shortUrlService.CreateShortUrlAsync(request.FullUrl, userId.Value, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { error = result.Error ?? result.Message ?? "Failed to create short URL" });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteShortUrl(int id, CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var isAdmin = User.IsInRole(AuthorizationConsts.Roles.Admin.Name);

        var result = await _shortUrlService.DeleteShortUrlAsync(id, userId.Value, isAdmin, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { message = result.Message ?? "Short URL deleted successfully" });
        }

        return BadRequest(new { error = result.Message ?? result.Message ?? "Failed to delete URL" });
    }

    [HttpGet("check-exists")]
    [Authorize]
    public async Task<IActionResult> CheckUrlExists([FromQuery] string fullUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fullUrl))
        {
            return BadRequest(new { error = "Full URL is required" });
        }

        var userId = await GetCurrentUserIdAsync();

        var result = await _shortUrlService.CheckUrlExistsAsync(fullUrl, userId, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { error = result.Error ?? result.Message ?? "Failed to check URL existence" });
    }

    private async Task<int?> GetCurrentUserIdAsync()
    {
        if (!User.Identity?.IsAuthenticated == true)
        {
            return null;
        }

        var user = await _userManager.GetUserAsync(User);
        return user?.Id;
    }
}

public class CreateShortUrlRequest
{
    public string FullUrl { get; set; } = null!;
}

