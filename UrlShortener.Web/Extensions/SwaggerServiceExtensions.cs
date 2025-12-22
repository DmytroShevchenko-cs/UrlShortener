namespace UrlShortener.Web.Extensions;

using Microsoft.OpenApi.Models;
using Shared.Common.Constants;

public static class SwaggerServiceExtensions
{
    public static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName!.Replace('+', '.'));

            options.SwaggerDoc(SwaggerConsts.AdminDocName,
                new OpenApiInfo { Title = SwaggerConsts.AdminDocName, Version = SwaggerConsts.UserDocName });
            options.SwaggerDoc(SwaggerConsts.UserDocName,
                new OpenApiInfo { Title = SwaggerConsts.UserDocName, Version = SwaggerConsts.UserDocName });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Bearer token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}