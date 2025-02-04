namespace Esport.Domain;

using Models;

public interface IEsportService
{
    /// <summary>
    /// Получение списка всех событий
    /// </summary>
    /// <returns>Список событий</returns>
    Task<IEnumerable<EsportEvent>> GetAllEsportEvents();
    
    /// <summary>
    /// Получение информации по конкретному событию
    /// </summary>
    /// <param name="eventId">Id события</param>
    /// <returns>Информация о событии</returns>
    Task<EsportEvent> GetEsportEventById(Guid eventId);
}