using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PostService.Domain.Events;

namespace PostService.Infrastructure.EventHandling;

public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly EventHandlerDispatcher _dispatcher;

    private readonly string _brokerList;
    private readonly List<string> _topics;
    private readonly string _groupId;

    public KafkaConsumerBackgroundService(
        EventHandlerDispatcher dispatcher,
        string brokerList,
        string groupId,
        ILogger<KafkaConsumerBackgroundService> logger, IConfiguration configuration)
    {
        _logger = logger;

        _dispatcher = dispatcher;
        _brokerList = brokerList;
        _groupId = groupId;
        _configuration = configuration;

        _topics = new List<string>
        {
            _configuration["Kafka:Topics:CategoryEvents"]
        };

        _logger.LogInformation($"==> Topics: {string.Join(", ", _topics)}");

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {

            var config = new ConsumerConfig
            {
                BootstrapServers = _brokerList,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Subscribe(_topics);

                _logger.LogInformation("==> Kafka consumer service started");

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));

                            if (consumeResult is null)
                                continue;


                            _logger.LogInformation($"==> Consume Result: {consumeResult.Message.Key} - {consumeResult.Message.Value}");


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

                        await Task.Delay(100, stoppingToken);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogInformation("==> Kafka consumer service is stopping.");
                }
                finally
                {
                    consumer.Close();
                }

            }
        });
    }

    private Type GetEventTypeFromMessage(string message)
    {
        _logger.LogInformation($"==> Message: {message}");

        var messageType = JsonDocument.Parse(message).RootElement.GetProperty("Type").GetString();


        _logger.LogInformation($"==> Message Type: {message}");

        return messageType switch
        {
            nameof(CategoryDeletedEvent) => typeof(CategoryDeletedEvent),
            _ => throw new InvalidOperationException($"==> Unknown event type: {messageType}")
        };
    }
}
