namespace Esport.Application.Dtos;

public class LeagueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EsportId { get; set; }
    public EsportDto Esport { get; set; } = null!;
    public ICollection<ChampionshipDto> Championships { get; set; } = new List<ChampionshipDto>();
}
