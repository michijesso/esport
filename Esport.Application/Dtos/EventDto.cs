namespace Esport.Application.Dtos;

public class EventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ChampionshipId { get; set; }
    public DateOnly EventDate { get; set; }
    public TimeOnly EventTime { get; set; }
    public ChampionshipDto Championship { get; set; } = null!;
    public ICollection<MarketDto> Markets { get; set; } = new List<MarketDto>();
    public ICollection<EventParticipantDto> EventParticipants { get; set; } = new List<EventParticipantDto>();
}
