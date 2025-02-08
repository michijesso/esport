namespace Esport.Kafka.Subscriber;

using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Common;
using Domain.Models;
using Domain;

public class KafkaSubscriberService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaSubscriberService> _logger;
    private readonly KafkaConfiguration _configuration;
    private readonly IEsportRepository _esportRepository;
    private readonly HttpClient _httpClient;

    public KafkaSubscriberService(KafkaConfiguration configuration,
                                    ILogger<KafkaSubscriberService> logger,
                                    IEsportRepository esportRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _esportRepository = esportRepository;
        _httpClient = new HttpClient();

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration.BootstrapServers,
            GroupId = _configuration.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(_configuration.Topic);
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
            if (isExistedEvent)
            {
                await _httpClient.GetAsync($"http://localhost:5088/api/notifications/getAllEvents");
            }
            else
            {
                await _httpClient.GetAsync($"http://localhost:5088/api/notifications/getEventById/{data.Event.Id}");
            }
        }
        _logger.LogInformation($"Processed message: {data}");
    }

    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}