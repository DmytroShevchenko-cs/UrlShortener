namespace UrlShortener.Web.Constraints;

public class ShortUrlCodeConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value) && value is string code)
        {
            return code.Length == 8 && System.Text.RegularExpressions.Regex.IsMatch(code, "^[A-Za-z0-9]+$");
        }
        return false;
    }
}

