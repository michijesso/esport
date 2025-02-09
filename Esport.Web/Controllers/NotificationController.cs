namespace Esport.Web.Controllers;

using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Dtos;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IWebSocketSpecifiedEventService _webSocketSpecifiedEventService;
    private readonly IWebSocketAllEventsService _webSocketAllEventsService;
    private readonly IEsportRepository _esportRepository;
    private readonly IMapper _mapper;

    public NotificationController(IWebSocketSpecifiedEventService webSocketSpecifiedEventService,
                                    IWebSocketAllEventsService webSocketAllEventsService,
                                    IEsportRepository esportRepository,
                                    IMapper mapper)
    {
        _webSocketSpecifiedEventService = webSocketSpecifiedEventService;
        _webSocketAllEventsService = webSocketAllEventsService;
        _esportRepository = esportRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("getEventById/{id}")]
    public async Task<IActionResult> GetEventById([FromRoute] int id)
    {
        var esportEvent = _esportRepository.GetByIdAsync(id);
        var mappedEsportEvent = _mapper.Map<EsportEventDto>(esportEvent);
        await _webSocketSpecifiedEventService.BroadcastSpecifiedEventAsync(JsonSerializer.Serialize(mappedEsportEvent), id);
        return Ok(esportEvent);
    }
    
    [HttpGet]
    [Route("getAllEvents")]
    public async Task<IActionResult> GetAllEvents()
    {
        var esportEvents = await _esportRepository.GetAllAsync();
        var mappedEsportEvents = _mapper.Map<IEnumerable<EsportEventDto>>(esportEvents);
        await _webSocketAllEventsService.BroadcastAllEventsAsync(JsonSerializer.Serialize(mappedEsportEvents));
        return Ok(esportEvents);
    }
}