using Microsoft.AspNetCore.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using UrlShortener.Web.Extensions;

namespace UrlShortener.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateApplicationBuilder(args);
        
        builder.Services
            .RegisterConfigurations(builder.Configuration)
            .RegisterInfrastructureServices(builder.Configuration)
            .RegisterDatabaseAccess(builder.Configuration)
            .RegisterCustomServices()
            .RegisterIdentity(builder.Configuration)
            .RegisterSwagger();
        
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
        var app = builder.Build();
        
        app.UseStaticFiles();
        app.UseForwardedHeaders();
        app.UseExceptionHandler(_ => { });
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseConfiguredSwagger();
        
        app.MapControllers();
        
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                
                context.Response.StatusCode = context.Features.Get<IExceptionHandlerFeature>()?.Error switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var errorResponse = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                        StackTrace = app.Environment.IsDevelopment() ? contextFeature.Error.StackTrace : null
                    };

                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
            });
        });
        
        await app.ExecuteStartupActions();
        await app.RunAsync();
    }
    
    private static WebApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder;
    }
}