namespace UrlShortener.Web.Extensions;

using DAL.Database.Seed;
using DAL.Services.DatabaseMigrationService;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Shared.Common.Configurations;
using Shared.Common.Constants;

public static class WebApplicationExtensions
{
    public static async Task ExecuteStartupActions(this WebApplication app)
    {
        var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();
        var serviceProvider = serviceScopeFactory!.CreateScope().ServiceProvider;
        
        var migrationService = serviceProvider.GetRequiredService<IDatabaseMigrationService>();
        await migrationService.MigrateAsync();
        
        var adminOptions = serviceProvider.GetRequiredService<IOptions<AdminConfig>>().Value;
        await DatabaseSeed.SeedDefaultAdminUserAsync(serviceProvider, adminOptions);
    }

    public static void UseConfiguredSwagger(this WebApplication app)
    {
        app.UseRouting();
        
        app.UseCors(SwaggerConsts.CorsPolicy);
        
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "/swagger/{documentName}.json";
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var openApiProperty = swaggerDoc.GetType().GetProperty("OpenApi");
                if (openApiProperty != null && openApiProperty.CanWrite)
                {
                    openApiProperty.SetValue(swaggerDoc, "3.0.1");
                }

                swaggerDoc.Servers =
                [
                    new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" },
                ];
            });
        })
        .UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";

            options.SwaggerEndpoint(
                $"/swagger/{SwaggerConsts.AdminDocName}.json",
                SwaggerConsts.AdminDocName);
            
            options.SwaggerEndpoint(
                $"/swagger/{SwaggerConsts.UserDocName}.json",
                SwaggerConsts.UserDocName);

            options.DefaultModelExpandDepth(2);
            options.DisplayRequestDuration();
            options.EnableValidator();
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
    }
}