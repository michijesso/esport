namespace Esport.Web.Dtos;

public class MarketDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<SelectionDto> Selections { get; set; } = new();
}