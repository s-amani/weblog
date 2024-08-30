using PostService.Domain.Events;

namespace PostService.Domain.Interfaces;

public interface IEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}