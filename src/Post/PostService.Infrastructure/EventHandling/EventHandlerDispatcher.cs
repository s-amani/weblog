using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
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
        await using var scope = _serviceProvider.CreateAsyncScope();

        var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        foreach (dynamic handler in handlers)
        {
            await handler.HandleAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}
