namespace Esport.Application.Dtos;

public class EsportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<LeagueDto> Leagues { get; set; } = new List<LeagueDto>();
}
