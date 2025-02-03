namespace Esport.Infrastructure;

using Domain;
using Microsoft.EntityFrameworkCore;

public class EsportRepository<T>(EsportDbContext context) : IEsportRepository<T>
    where T : class
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    // Получить сущность по ID
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    // Получить все сущности
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    // Добавить сущность
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    // Обновить сущность
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await context.SaveChangesAsync();
    }

    // Удалить сущность по ID
    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}