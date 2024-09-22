using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostService.Domain.Aggregates;

namespace PostService.Infrastructure.Persistence.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Author)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.OwnsOne(p => p.Content, cb =>
        {
            cb
            .Property(c => c.Text)
            .IsRequired()
            .HasColumnName("ContentText");

            cb
            .Property(c => c.Images)
            .HasConversion(
                images => JsonSerializer.Serialize(images, new JsonSerializerOptions()),
                images => JsonSerializer.Deserialize<List<string>>(images, new JsonSerializerOptions())
            )
            .HasColumnName("ContentImages");
        });

        builder.OwnsMany(p => p.Tags, cb =>
        {
            cb.Property(c => c.Name)
                .IsRequired()
                .HasColumnName("Tags");
        });

        builder.OwnsMany(p => p.Comments, cb =>
        {
            cb.Property(c => c.Content)
                .IsRequired()
                .HasColumnName("Tags");
        });
    }
}
