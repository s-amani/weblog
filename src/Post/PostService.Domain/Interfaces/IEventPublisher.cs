namespace PostService.Domain.Events.Interface;

public interface IEventPublisher
{
    Task Publish<T>(T eventToPublish) where T : class;
}
