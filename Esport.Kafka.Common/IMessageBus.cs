namespace Esport.Kafka.Common;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string topic);
}