namespace PostService.Infrastructure.Persistence;

using System;
using System.Collections.Generic;
using System.Linq;
using PostService.Domain.Aggreagates;
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
        Guid categoryId = default;

        if (!_context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category("General"),
                new Category("Tech")
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            categoryId = _context.Categories.FirstOrDefault()?.Id ?? default;
        }

        if (!_context.Posts.Any())
        {
            // Seed Posts
            var posts = new List<Post>
                {
                    new Post{ Id = Guid.NewGuid(), CategoryId = categoryId, Title = "First Post", Content = new PostContent{ Text = "This is the first post content" } },
                    new Post{ Id = Guid.NewGuid(), CategoryId = categoryId, Title = "Second Post", Content = new PostContent{ Text = "This is the second post content" } }
                };

            _context.Posts.AddRange(posts);
            _context.SaveChanges();
        }
    }
}