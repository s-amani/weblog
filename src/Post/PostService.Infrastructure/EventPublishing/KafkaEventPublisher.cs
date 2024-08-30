using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using PostService.Domain.Events.Interface;

namespace PostService.Infrastructure.EventPublishing;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly string _topic;

    public KafkaEventPublisher(string brokerList, string topic, ILogger<KafkaEventPublisher> logger)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            EnableIdempotence = true, 
            Acks = Acks.All,          
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _topic = topic;
        _logger = logger;
    }

    public async Task Publish<T>(T eventToPublish) where T : class
    {
        var key = Guid.NewGuid().ToString(); // Generate a unique key for the message
        var value = JsonSerializer.Serialize(eventToPublish); // Serialize the event to JSON

        try
        {
            var result = await _producer.ProduceAsync(_topic, new Message<string, string> { Key = key, Value = value });

            _logger.LogInformation($"==> Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
        }
        catch (ProduceException<string, string> e)
        {
            _logger.LogInformation($"==> Delivery failed: {e.Error.Reason}");
            throw; 
        }
    }
}
