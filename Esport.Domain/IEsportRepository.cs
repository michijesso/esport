namespace Esport.Domain;

using Models;

public interface IEsportRepository
{
    /// <summary>
    /// Получение сущносит по Id
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    EsportEvent GetByIdAsync(int id);
    
    /// <summary>
    /// Получение всех сущностей
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<EsportEvent>> GetAllAsync();
    
    /// <summary>
    /// Добавить сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task<bool> AddOrUpdateAsync(EsportEvent entity);
    
    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">Сущность</param>
    /// <returns></returns>
    Task DeleteAsync(int id);
}