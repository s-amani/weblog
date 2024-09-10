using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostService.Domain.Events.Interface;

namespace PostService.Infrastructure.EventPublishing;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly Dictionary<string, string> _topicMappings;
    private readonly IConfiguration _configuration;

    public KafkaEventPublisher(
        string brokerList,
        string topic,
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

        // Load topic mappings from configuration
        _topicMappings = new Dictionary<string, string>
        {
            { "CategoryDeleted", _configuration["Kafka:Topics:CategoryEvents"] }
        };

        _logger = logger;
        _configuration = configuration;
    }

    public async Task Publish<T>(T eventToPublish, string eventType) where T : class
    {
        var key = Guid.NewGuid().ToString(); // Generate a unique key for the message
        var value = JsonSerializer.Serialize(eventToPublish); // Serialize the event to JSON

        try
        {
            if (_topicMappings.TryGetValue(eventType, out string topic))
            {
                topic = GetTopicName(eventType);
                
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

    public string GetTopicName(string eventType)
    {
        // Example pattern: dev-{eventType.ToLower()}-events
        return $"dev-{eventType.ToLower()}-events";
    }
}
