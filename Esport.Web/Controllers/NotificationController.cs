namespace Esport.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using Domain;
using Domain.Models;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;
    private readonly IEsportRepository<EsportEvent> _esportRepository;

    public NotificationController(IWebSocketService webSocketService, IEsportRepository<EsportEvent> esportRepository)
    {
        _webSocketService = webSocketService;
        _esportRepository = esportRepository;
    }

    [HttpGet]
    [Route("getEventById/{id}")]
    public async Task<IActionResult> GetEventById([FromRoute] Guid id)
    {
        var esportEvent = await _esportRepository.GetByIdAsync(id);
        await _webSocketService.BroadcastMessageAsync(JsonSerializer.Serialize(esportEvent), id);
        return Ok(esportEvent);
    }
    
    [HttpGet]
    [Route("getAllEvents")]
    public async Task<IActionResult> GetAllEvents()
    {
        var esportEvents = await _esportRepository.GetAllAsync();
        await _webSocketService.BroadcastMessageAsync(JsonSerializer.Serialize(esportEvents), null);
        return Ok(esportEvents);
    }
}
