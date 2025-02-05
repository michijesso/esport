namespace Esport.Web.Dtos;

public class EsportEventDto
{
    public string Esport { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string Championship { get; set; } = string.Empty;
    public EventDto Event { get; set; } = new();
}