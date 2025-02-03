namespace Esport.GeneratorService.Core.Models;

public class EsportEvent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CurrentScore { get; set; }
    public List<EsportParticipant> Participants { get; set; }
    public EsportMarket Market { get; set; }
}