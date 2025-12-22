using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Web.Controllers;

public class AboutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}

