namespace UrlShortener.Web.Extensions;

using DAL.Database;
using DAL.Database.Entities.Identity;
using Microsoft.AspNetCore.Identity;
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

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
        });
        
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