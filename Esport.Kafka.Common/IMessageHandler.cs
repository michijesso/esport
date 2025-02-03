namespace Esport.Kafka.Common;

public interface IMessageHandler<T>
{
    Task HandleAsync(T message);
}