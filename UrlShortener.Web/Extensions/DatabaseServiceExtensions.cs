namespace UrlShortener.Web.Extensions;

using DAL.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection RegisterDatabaseAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<BaseDbContext>(o => o.UseNpgsql(
            connectionString!,
            b =>
            {
                b.MigrationsAssembly("UrlShortener.DAL");
                b.UseNodaTime();
            })
        );

        services.AddSingleton(_ =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UseNodaTime();
            var dataSource = dataSourceBuilder.Build();

            return dataSource;
        });

        return services;
    }
}