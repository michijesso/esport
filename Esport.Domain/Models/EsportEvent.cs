namespace Esport.Domain.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("esport_events", Schema = "public")]
public class EsportEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("esport")]
    public string Esport { get; set; }

    [Column("league")]
    public string League { get; set; }

    [Column("championship")]
    public string Championship { get; set; }

    public Event Event { get; set; }
}