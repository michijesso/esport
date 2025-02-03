namespace Esport.Application.Dtos;

public class ChampionshipDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LeagueId { get; set; }
    public LeagueDto League { get; set; } = null!;
    public ICollection<EventDto> Events { get; set; } = new List<EventDto>();
}
