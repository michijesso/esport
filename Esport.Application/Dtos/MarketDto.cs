namespace Esport.Application.Dtos;

public class MarketDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EventId { get; set; }
    public EventDto Event { get; set; } = null!;
    public ICollection<SelectionDto> Selections { get; set; } = new List<SelectionDto>();
}