namespace Esport.Web.Controllers;

using System.Net.WebSockets;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/")]
public class EsportController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;

    public EsportController(IWebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }

    [HttpGet("ws/getEventById/{eventId}")]
    public async Task WebSocketConnect([FromRoute] int eventId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketService.HandleWebSocketForSpecifiedEventAsync(webSocket, eventId);
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
            await _webSocketService.HandleWebSocketForAllEventsAsync(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}