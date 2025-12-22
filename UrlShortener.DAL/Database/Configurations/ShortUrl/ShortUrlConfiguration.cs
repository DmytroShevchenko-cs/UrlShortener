namespace UrlShortener.DAL.Database.Configurations.ShortUrl;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities.ShortUrls;

public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.Property(s => s.FullUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(s => s.ShortUrlCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.UserId)
            .IsRequired();
        
        builder.HasIndex(s => s.ShortUrlCode)
            .IsUnique();
        
        builder.HasIndex(s => s.UserId);
        
        builder.HasOne(s => s.User)
            .WithMany(u => u.ShortUrls)
            .HasForeignKey(s => s.UserId);
    }
}

