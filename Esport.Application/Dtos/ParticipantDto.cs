namespace Esport.Application.Dtos;

public class ParticipantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<EventParticipantDto> EventParticipants { get; set; } = new List<EventParticipantDto>();
}
