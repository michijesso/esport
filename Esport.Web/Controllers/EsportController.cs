namespace Esport.Web.Controllers;

using System.Net.WebSockets;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/")]
public class EsportController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IWebSocketService _webSocketService;

    public EsportController(IMapper mapper, IWebSocketService webSocketService)
    {
        _mapper = mapper;
        _webSocketService = webSocketService;
    }

    [HttpGet("ws")]
    public async Task<IActionResult> WebSocketConnect()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSocketService.AddSocket(Guid.NewGuid(), webSocket);
            await _webSocketService.HandleWebSocketAsync(webSocket);
            return Ok();
        }
        else
        {
            return BadRequest("WebSocket connection required");
        }
    }
}