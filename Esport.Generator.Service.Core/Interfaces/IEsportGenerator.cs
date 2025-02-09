namespace Esport.GeneratorService.Core.Interfaces;

using Models;

public interface IEsportGenerator
{
    EsportGeneratorModel GenerateEsportData();
    EsportGeneratorModel UpdateEsportData(EsportGeneratorModel esportGeneratorModel);
}