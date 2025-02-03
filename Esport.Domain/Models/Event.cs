namespace Esport.Domain.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("events", Schema = "public")]
public class Event
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("current_score")]
    public string CurrentScore { get; set; }

    public List<Participant> Participants { get; set; }

    public Market Market { get; set; }
}