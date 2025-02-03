namespace Esport.GeneratorService.Core.Interfaces;

using Models;

public interface IEsportSender
{
    Task SendAsync(EsportGeneratorModel model);
}