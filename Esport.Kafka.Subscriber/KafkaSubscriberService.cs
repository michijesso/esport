namespace Esport.Kafka.Subscriber;

using System.Text.Json;
using Common;
using Confluent.Kafka;
using Domain;
using Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

public class KafkaSubscriberService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaSubscriberService> _logger;
    private readonly IEsportRepository _esportRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiConnectionConfiguration _apiConnection;

    public KafkaSubscriberService(IOptions<KafkaConfiguration> kafkaConfig,
                                    ILogger<KafkaSubscriberService> logger,
                                    IEsportRepository esportRepository,
                                    IHttpClientFactory httpClientFactory,
                                    IOptions<ApiConnectionConfiguration> apiConnection)
    {
        _logger = logger;
        _esportRepository = esportRepository;
        _httpClientFactory = httpClientFactory;
        _apiConnection = apiConnection.Value;

        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConfig.Value.BootstrapServers,
            GroupId = kafkaConfig.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(kafkaConfig.Value.Topic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka subscriber started");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var message = consumeResult.Message.Value;

                    _logger.LogInformation($"Received message: {message}");

                    await ProcessMessageAsync(message);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Kafka consuming was cancelled");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
            }
        }
        finally
        {
            _consumer.Close();
        }

        _logger.LogInformation("Kafka subscriber stopped");
    }

    private async Task ProcessMessageAsync(string message)
    {
        var data = JsonSerializer.Deserialize<EsportEvent>(message);
        if (data != null)
        {
            var isExistedEvent = await _esportRepository.AddOrUpdateAsync(data);

            var client = _httpClientFactory.CreateClient();
            var endpoint = isExistedEvent ? "getAllEvents" : $"getEventById/{data.Event.Id}";
            var response = await client.GetAsync($"{_apiConnection.BaseUrl}{endpoint}");

            _logger.LogInformation($"Notification sent: {response.StatusCode}");
        }
        _logger.LogInformation($"Processed message: {data}");
    }

    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}