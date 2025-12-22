namespace UrlShortener.Web.Extensions;

using System.Security.Claims;
using System.Text;
using DAL.Database;
using DAL.Database.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Common.Constants;

public static class IdentityServiceExtensions
{
    public static IServiceCollection RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<BaseDbContext>()
            .AddDefaultTokenProviders();

        var secretKey = configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not found");
        var key = Encoding.UTF8.GetBytes(secretKey);
        
        // services.AddAuthentication(options =>
        //     {
        //         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     })
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(key),
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //             ClockSkew = TimeSpan.Zero,
        //             RoleClaimType = ClaimTypes.Role
        //         };
        //     });


        services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationConsts.Policies.Admin,
                policyUser =>
                {
                    policyUser.RequireRole(AuthorizationConsts.Roles.Admin.Name);
                });

            options.AddPolicy(AuthorizationConsts.Policies.User,
                policyUser =>
                {
                    policyUser.RequireRole(AuthorizationConsts.Roles.Admin.Name, AuthorizationConsts.Roles.User.Name);
                });
        });

        return services;
    }
}