using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.BLL.Services.ShortUrls;

namespace UrlShortener.Web.Controllers;

public class ShortUrlsController : Controller
{
    private readonly IShortUrlService _shortUrlService;

    public ShortUrlsController(
        IShortUrlService shortUrlService)
    {
        _shortUrlService = shortUrlService;
    }

    [HttpGet]
    [Route("ShortUrls/info/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Info(int id, CancellationToken cancellationToken)
    {
        var result = await _shortUrlService.GetShortUrlByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            return View(result.Value);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("{code:shortUrlCode}")]
    public async Task<IActionResult> Redirect(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return NotFound();
        }

        var result = await _shortUrlService.GetShortUrlByCodeAsync(code, cancellationToken);

        if (result.IsSuccess && result.Value != null)
        {
            return Redirect(result.Value.FullUrl);
        }

        return NotFound();
    }
}

