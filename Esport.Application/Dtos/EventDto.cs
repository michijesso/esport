namespace Esport.Application.Dtos;

public class EventDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CurrentScore { get; set; } = string.Empty;
    public List<ParticipantDto> Participants { get; set; } = new();
    public MarketDto Market { get; set; } = new();
}