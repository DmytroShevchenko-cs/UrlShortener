namespace UrlShortener.DAL.Database;

using Entities.Identity;
using Entities.ShortUrls;
using Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Seed;

public class BaseDbContext(DbContextOptions<BaseDbContext> options)
    : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{

    public DbSet<ShortUrl> ShortUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
        
        modelBuilder.AddPostgreSqlRules();
        modelBuilder.AddIdentityRules();
        modelBuilder.SeedLatest();
    }
}

