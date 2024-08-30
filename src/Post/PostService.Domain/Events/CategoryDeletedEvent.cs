namespace PostService.Domain.Events;

public class CategoryDeletedEvent : IDomainEvent
{
    public Guid CategoryId { get; }

    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOn => DateTime.UtcNow;

    public CategoryDeletedEvent(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}
