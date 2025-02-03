namespace Esport.GeneratorService.Implementations;

using Core.Interfaces;
using Esport.GeneratorService.Core.Models;
using Kafka.Common;
using Kafka.Publisher;
using Microsoft.Extensions.Configuration;

public class EsportSender : IEsportSender
{
    private readonly IConfiguration _configuration;

    public EsportSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendAsync(EsportGeneratorModel model)
    {
        var kafkaConfiguration = _configuration.GetSection("Kafka").Get<KafkaConfiguration>();
        
        try
        {
            if (kafkaConfiguration != null)
            {
                var eventPublisher = new KafkaMessageBus(kafkaConfiguration);

                await eventPublisher.PublishAsync(model, kafkaConfiguration.Topic);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }
}