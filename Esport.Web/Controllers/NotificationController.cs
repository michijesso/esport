using AutoMapper;
using Esport.Web.Dtos;

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
    private readonly IEsportRepository _esportRepository;
    private readonly IMapper _mapper;

    public NotificationController(IWebSocketService webSocketService, IEsportRepository esportRepository, IMapper mapper)
    {
        _webSocketService = webSocketService;
        _esportRepository = esportRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("getEventById/{id}")]
    public async Task<IActionResult> GetEventById([FromRoute] int id)
    {
        var esportEvent = _esportRepository.GetByIdAsync(id);
        var mappedEsportEvent = _mapper.Map<EsportEventDto>(esportEvent);
        await _webSocketService.BroadcastSpecifiedEventAsync(JsonSerializer.Serialize(mappedEsportEvent), id);
        return Ok(esportEvent);
    }
    
    [HttpGet]
    [Route("getAllEvents")]
    public async Task<IActionResult> GetAllEvents()
    {
        var esportEvents = await _esportRepository.GetAllAsync();
        var mappedEsportEvents = _mapper.Map<IEnumerable<EsportEventDto>>(esportEvents);
        await _webSocketService.BroadcastAllEventsAsync(JsonSerializer.Serialize(mappedEsportEvents));
        return Ok(esportEvents);
    }
}