namespace Esport.Kafka.Publisher;

using System.Text.Json;
using Common;
using Confluent.Kafka;

public class KafkaMessageBus : IMessageBus, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    
    public KafkaMessageBus(KafkaConfiguration configuration)
    {
        var config = new ProducerConfig { BootstrapServers = configuration.BootstrapServers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }
    
    public async Task PublishAsync<T>(T message, string topic)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
            throw;
        }
    }
    
    public void Dispose()
    {
        _producer.Dispose();
    }
}