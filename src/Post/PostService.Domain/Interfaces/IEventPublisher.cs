namespace PostService.Domain.Events.Interface;

public interface IEventPublisher
{
    Task Publish<T>(T eventToPublish, string eventType) where T : class;
}
