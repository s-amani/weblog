using Microsoft.Extensions.DependencyInjection;
using PostService.Domain.Events;
using PostService.Domain.Interfaces;

namespace PostService.Infrastructure.EventHandling;

public class EventHandlerDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventHandlerDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken) where TEvent : IDomainEvent
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (dynamic handler in handlers)
        {
            await handler.HandleAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}
