namespace PostService.Infrastructure.Persistence;

using System;
using System.Collections.Generic;
using System.Linq;
using PostService.Domain.Aggregates;
using PostService.Domain.ValueObjects;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (!_context.Posts.Any())
        {
            // Seed Posts
            var posts = new List<Post>
                {
                    new Post{ Id = Guid.NewGuid(), Title = "First Post", Content = new PostContent{ Text = "This is the first post content" } },
                    new Post{ Id = Guid.NewGuid(), Title = "Second Post", Content = new PostContent{ Text = "This is the second post content" } }
                };

            _context.Posts.AddRange(posts);
            _context.SaveChanges();
        }
    }
}