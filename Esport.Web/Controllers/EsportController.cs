namespace Esport.Web.Controllers;

using Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/")]
public class EsportController : ControllerBase
{
    private readonly IWebSocketSpecifiedEventService _webSocketSpecifiedEventService;
    private readonly IWebSocketAllEventsService _webSocketAllEventsService;

    public EsportController(IWebSocketSpecifiedEventService webSocketSpecifiedEventService, IWebSocketAllEventsService webSocketAllEventsService)
    {
        _webSocketSpecifiedEventService = webSocketSpecifiedEventService;
        _webSocketAllEventsService = webSocketAllEventsService;
    }

    [HttpGet("ws/getEventById/{eventId}")]
    public async Task WebSocketConnect([FromRoute] int eventId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketSpecifiedEventService.HandleWebSocketForSpecifiedEventAsync(webSocket, eventId);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
    
    [HttpGet("ws/getAllEvents")]
    public async Task WebSocketConnect()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketAllEventsService.HandleWebSocketForAllEventsAsync(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}