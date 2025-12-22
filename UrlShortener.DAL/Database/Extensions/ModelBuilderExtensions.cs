namespace UrlShortener.DAL.Database.Extensions;

using Entities.Identity;
using Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static void AddPostgreSqlRules(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (entity.BaseType == null)
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToSnakeCase());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
            }
        }
    }
    public static void AddIdentityRules(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(i => { i.ToTable(nameof(BaseDbContext.Users).ToSnakeCase()); });
        modelBuilder.Entity<Role>(i => { i.ToTable(nameof(BaseDbContext.Roles).ToSnakeCase()); });
        modelBuilder.Entity<UserRole>(i => { i.ToTable(nameof(BaseDbContext.UserRoles).ToSnakeCase()); });
        modelBuilder.Entity<UserLogin>(i => { i.ToTable(nameof(BaseDbContext.UserLogins).ToSnakeCase()); });
        modelBuilder.Entity<RoleClaim>(i => { i.ToTable(nameof(BaseDbContext.RoleClaims).ToSnakeCase()); });
        modelBuilder.Entity<UserClaim>(i => { i.ToTable(nameof(BaseDbContext.UserClaims).ToSnakeCase()); });
        modelBuilder.Entity<UserToken>(i => { i.ToTable(nameof(BaseDbContext.UserTokens).ToSnakeCase()); });
        
        modelBuilder.Entity<UserRole>(userRole =>
        {
            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

            userRole.HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            userRole.HasOne(x => x.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        });
    }
}