namespace UrlShortener.DAL.Database.Configurations.User;

using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(r => r.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.UserName).IsUnique();
        
        builder.Property(r => r.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(r => r.Email).IsUnique();
    }
}