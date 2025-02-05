using System.Net.WebSockets;
using AutoMapper;
using Esport.Domain;
using Esport.Domain.Models;
using Esport.Web;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EsportController : ControllerBase
{
    private readonly IEsportRepository<EsportEvent> _esportRepository;
    private readonly IMapper _mapper;
    private readonly IWebSocketService _webSocketService;

    public EsportController(IEsportRepository<EsportEvent> esportRepository, IMapper mapper, IWebSocketService webSocketService)
    {
        _esportRepository = esportRepository;
        _mapper = mapper;
        _webSocketService = webSocketService;
    }

    [HttpGet("getAllEvents")]
    public async Task<IActionResult> GetAllEvents()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSocketService.AddSocket(Guid.NewGuid(), webSocket);
            await _webSocketService.HandleWebSocketAsync(webSocket);
            return new EmptyResult();
        }
        else
        {
            return BadRequest("WebSocket connection required");
        }
    }

    [HttpGet("getEventById/{id}")]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSocketService.AddSocket(Guid.NewGuid(), webSocket);
            await _webSocketService.HandleWebSocketAsync(webSocket);
            return new EmptyResult(); // Возвращаем пустой результат для WebSocket-запросов
        }
        else
        {
            return BadRequest("WebSocket connection required");
        }
    }
}