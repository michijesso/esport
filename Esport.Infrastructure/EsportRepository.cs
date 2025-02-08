namespace Esport.Infrastructure;

using Domain;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

public class EsportRepository : IEsportRepository
{
    private readonly EsportDbContext _context;

    public EsportRepository(EsportDbContext context)
    {
        _context = context;
    }
    
    public EsportEvent GetByIdAsync(int id)
    {
        return _context.EsportEvents
            .Include(e => e.Event)
                .ThenInclude(p => p.Participants)
            .Include(e => e.Event)
                .ThenInclude(m => m.Market)
                .ThenInclude(s => s.Selections)
            .FirstOrDefault(x => x.Event.Id == id) ?? throw new KeyNotFoundException();
    }

    public async Task<IEnumerable<EsportEvent>> GetAllAsync()
    {
        return await _context.EsportEvents
            .Include(e => e.Event)
                .ThenInclude(p => p.Participants)
            .Include(e => e.Event)
                .ThenInclude(m => m.Market)
                .ThenInclude(s => s.Selections)
            .ToListAsync();
    }

    public async Task<bool> AddOrUpdateAsync(EsportEvent entity)
    {
        var existingEntity = await _context.Events.FindAsync(entity.Event.Id);

        if (existingEntity == null)
        {
            await _context.EsportEvents.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            existingEntity.CurrentScore = entity.Event.CurrentScore;
            await _context.SaveChangesAsync();
            return false;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.EsportEvents.FindAsync(id);
        if (entity != null)
        {
            _context.EsportEvents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}