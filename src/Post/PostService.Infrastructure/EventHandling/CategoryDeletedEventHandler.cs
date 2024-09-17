using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;
using PostService.Domain.Events;
using PostService.Domain.Interfaces;
using PostService.Domain.Repositories;

namespace PostService.Infrastructure.EventHandling;

public class CategoryDeletedEventHandler : IEventHandler<CategoryDeletedEvent>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CategoryDeletedEventHandler> _logger;

    public CategoryDeletedEventHandler(IPostRepository postRepository, IUnitOfWork uow, ILogger<CategoryDeletedEventHandler> logger)
    {
        _uow = uow;
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CategoryDeletedEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"==> Handling {domainEvent.Type} event");
            
            // Fetch and delete all posts associated with the deleted category
            var posts = _postRepository.GetPostsByCategoryId(domainEvent.CategoryId);

            foreach (var post in posts)
            {
                _postRepository.Remove(post);
            }

            // Commit the transaction
            await _uow.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Handle exceptions, potentially retry or log
            _logger.LogError($"==> Error consuming message: {ex.Message}");
            throw;
        }
    }
}
