namespace Esport.GeneratorService.Core.Models;

public class EsportMarket
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<EsportSelection> Selections { get; set; }
}