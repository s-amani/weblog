using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;
using PostService.Domain.Events;
using PostService.Domain.Interfaces;
using PostService.Domain.Repositories;

namespace PostService.Infrastructure.EventHandling;

public class CategoryDeletedEventHandler : IEventHandler<CategoryDeletedEvent>
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CategoryDeletedEventHandler> _logger;

    public CategoryDeletedEventHandler(
        IUnitOfWork uow, 
        ICategoryRepository repository, 
        ILogger<CategoryDeletedEventHandler> logger)
    {
        _uow = uow;
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(CategoryDeletedEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"==> Handling {domainEvent.Type} event");

            // Fetch and delete all posts associated with the deleted category
            var category = await _repository.Get(domainEvent.CategoryId);

            if (category is null)
                return;

            category.RemoveAssociatedPosts();

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
