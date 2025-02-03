namespace Esport.Application.Dtos;

public class EventParticipantDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public EventDto Event { get; set; } = null!;
    public int ParticipantId { get; set; }
    public ParticipantDto Participant { get; set; } = null!;
}
