namespace Esport.GeneratorService.Core.Models;

public class EsportGeneratorModel
{
    public string Esport { get; set; } 
    public string League { get; set; } 
    public string Championship { get; set; } 
    public EsportEvent Event { get; set; }
}