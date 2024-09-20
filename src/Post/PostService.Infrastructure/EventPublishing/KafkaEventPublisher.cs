using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostService.Domain.Events;
using PostService.Domain.Events.Interface;

namespace PostService.Infrastructure.EventPublishing;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly IConfiguration _configuration;

    public KafkaEventPublisher(
        string brokerList,
        ILogger<KafkaEventPublisher> logger,
        IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            EnableIdempotence = true,
            Acks = Acks.All,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();

        _logger = logger;
        _configuration = configuration;
    }

    public async Task Publish<T>(T eventToPublish, string eventType) where T : class
    {
        var key = Guid.NewGuid().ToString(); 
        var value = JsonSerializer.Serialize(eventToPublish); 

        try
        {
            _logger.LogInformation($"==> The event: {eventType} has been requested to be published to Kafka");

            var topic = GetTopicName(eventType);
            
            if (!string.IsNullOrEmpty(topic))
            {
                var result = await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });

                _logger.LogInformation($"==> Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");

                return;
            }

            throw new InvalidOperationException($"No topic configured for event type {eventType}");
        }
        catch (ProduceException<string, string> e)
        {
            _logger.LogInformation($"==> Delivery failed: {e.Error.Reason}");
            throw;
        }
    }

    public string GetTopicName(string eventType) => eventType switch
    {
        nameof(CategoryDeletedEvent) => _configuration["Kafka:Topics:CategoryEvents"],
        _ => throw new NotImplementedException()
    };
}
