namespace Esport.Application.Services;

using Domain;
using Domain.Models;

public class EsportService : IEsportService
{
    private readonly IEsportRepository<EsportEvent> _esportRepository;

    public EsportService(IEsportRepository<EsportEvent> esportRepository)
    {
        _esportRepository = esportRepository;
    }

    public Task<IEnumerable<EsportEvent>> GetAllEsportEvents()
    {
        throw new NotImplementedException();
    }

    public Task<EsportEvent> GetEsportEventById(Guid eventId)
    {
        throw new NotImplementedException();
    }
}