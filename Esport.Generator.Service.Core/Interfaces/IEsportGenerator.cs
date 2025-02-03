namespace Esport.GeneratorService.Core.Interfaces;

using Models;

public interface IEsportGenerator
{
    Task<EsportGeneratorModel> GenerateAsync();
}