using System.Text.Json;
using Esport.Domain;
using Esport.Domain.Models;

namespace Esport.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

    [HttpPost]
    public async Task<IActionResult> NotifyClients()
    {
        var esportEvents = await _esportRepository.GetAllAsync();
        await _webSocketService.BroadcastMessageAsync(JsonSerializer.Serialize(esportEvents));
        return Ok();
    }
}
