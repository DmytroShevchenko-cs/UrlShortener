namespace UrlShortener.Web.Extensions;

using BLL.Services.ShortUrls;
using DAL.Commands.ShortUrls.CreateShortUrl;
using DAL.Services.DatabaseMigrationService;

public static class CustomServiceExtensions
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
             options.RegisterServicesFromAssembly(typeof(DatabaseMigrationService).Assembly);
             options.RegisterServicesFromAssembly(typeof(CreateShortUrlCommand).Assembly);
        });
        
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
        services.AddScoped<IShortUrlService, ShortUrlService>();
        
        return services;
    }
}