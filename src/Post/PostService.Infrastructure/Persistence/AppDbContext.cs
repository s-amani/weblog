using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PostService.Domain.Aggreagates;
using PostService.Domain.Aggregates;
using PostService.Domain.Entities;
using PostService.Domain.ValueObjects;
using PostService.Infrastructure.Persistence.Configuration;

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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryConfiguration).Assembly);
    }

}
