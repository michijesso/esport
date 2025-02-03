namespace Esport.Application.Dtos;

public class SelectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MarketId { get; set; }
    public decimal Odds { get; set; }
    public MarketDto Market { get; set; } = null!;
}
