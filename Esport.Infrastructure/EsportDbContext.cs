namespace Esport.Infrastructure;

using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class EsportDbContext : DbContext
{
    public EsportDbContext(DbContextOptions<EsportDbContext> options) : base(options)
    {
    }

    public DbSet<EsportEvent> EsportEvents { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Market> Markets { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Selection> Selections { get; set; }
}