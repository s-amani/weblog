using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PostService.Domain.Aggregates;
using PostService.Domain.Entities;
using PostService.Domain.ValueObjects;

namespace PostService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(x => x.Title)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(x => x.Author)
                .HasMaxLength(128)
                .IsRequired(false);

            entity.HasOne(x=> x.Category)
                .WithMany(x=> x.Posts)
                .IsRequired(false);

            entity.OwnsOne(p => p.Content, cb =>
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

            entity.OwnsMany(p => p.Tags, cb =>
            {
                cb.Property(c => c.Name)
                    .IsRequired()
                    .HasColumnName("Tags");
            });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasMany(x => x.Posts)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId);
        });
    }
}
