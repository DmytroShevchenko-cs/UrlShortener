namespace UrlShortener.Web.Extensions;

using DAL.Services.DatabaseMigrationService;

public static class CustomServiceExtensions
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
             options.RegisterServicesFromAssembly(typeof(DatabaseMigrationService).Assembly);
        });
        
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
        
        return services;
    }
}