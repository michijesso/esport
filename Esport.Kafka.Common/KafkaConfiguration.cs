namespace Esport.Kafka.Common;

public class KafkaConfiguration
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public string Topic { get; set; }
}