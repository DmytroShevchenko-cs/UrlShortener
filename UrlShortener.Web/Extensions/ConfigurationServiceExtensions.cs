namespace UrlShortener.Web.Extensions;

using Shared.Common.Configurations;

public static class ConfigurationServiceExtensions
{
    public static IServiceCollection RegisterConfigurations(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AdminConfig>(configuration.GetSection(nameof(AdminConfig)));

        return services;
    }
}