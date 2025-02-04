namespace Esport.Application.Dtos;

public class SportsEventDto
{
    public string Esport { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string Championship { get; set; } = string.Empty;
    public EventDto Event { get; set; } = new();
}