using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PostService.Domain.Events;

namespace PostService.Infrastructure.EventHandling;

public class KafkaConsumerBackgroundService: BackgroundService
{
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly EventHandlerDispatcher _dispatcher;

    private readonly string _brokerList;
    private readonly string _topic;
    private readonly string _groupId;

    public KafkaConsumerBackgroundService(EventHandlerDispatcher dispatcher, 
        string brokerList, string topic, string groupId, 
        ILogger<KafkaConsumerBackgroundService> logger)
    {
        _dispatcher = dispatcher;
        _brokerList = brokerList;
        _topic = topic;
        _groupId = groupId;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _brokerList,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    // Determine event type from the message
                    var eventType = GetEventTypeFromMessage(consumeResult.Message.Value);
                    var domainEvent = JsonSerializer.Deserialize(consumeResult.Message.Value, eventType);

                    if (domainEvent is IDomainEvent domainEventInstance)
                    {
                        await _dispatcher.DispatchAsync(domainEventInstance, stoppingToken);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"==> Error consuming message: {e.Error.Reason}");
                }
            }

            consumer.Close();
        }
    }

    private Type GetEventTypeFromMessage(string message)
    {
        // Implement logic to determine the event type from the message
        // This could be based on a field in the message or some other metadata
        // Example pseudo-code:
        var messageType = JsonDocument.Parse(message).RootElement.GetProperty("Type").GetString();

        return messageType switch
        {
            nameof(CategoryDeletedEvent) => typeof(CategoryDeletedEvent),
            _ => throw new InvalidOperationException($"==> Unknown event type: {messageType}")
        };
    }
}
