namespace Esport.GeneratorService.Core.Interfaces;

using Models;

public interface IEsportGenerator
{
    EsportGeneratorModel GenerateAsync();
    EsportGeneratorModel UpdateEsportData(EsportGeneratorModel esportGeneratorModel);
}