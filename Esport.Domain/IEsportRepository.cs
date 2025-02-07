namespace Esport.Domain;

public interface IEsportRepository<T>
{
    /// <summary>
    /// Получение сущносит по Id
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    Task<T> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Получение всех сущностей
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// Добавить сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Обновить сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task UpdateAsync(T entity);
    
    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">Сущность</param>
    /// <returns></returns>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Проверка существования сущность
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Guid id);
}