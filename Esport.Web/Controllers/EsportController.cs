using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Esport.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Esport.Web.Controllers;

[ApiController]
[Route("api/ws")]
public class EsportController : ControllerBase
{
    private static readonly ConcurrentDictionary<WebSocket, bool> _sockets = new();
    private readonly IEsportService _esportService;

    public EsportController(IEsportService esportService)
    {
        _esportService = esportService;
    }

    [HttpGet("connect")]
    public async Task Connect()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _sockets.TryAdd(webSocket, true);
            await HandleWebSocketCommunication(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    private async Task HandleWebSocketCommunication(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await ProcessWebSocketMessage(webSocket, buffer[..result.Count]);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
            }
        }
        finally
        {
            _sockets.TryRemove(webSocket, out _);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
        }
    }

    private async Task ProcessWebSocketMessage(WebSocket webSocket, byte[] message)
    {
        var request = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
        if (request == null || !request.TryGetValue("action", out var action)) return;

        switch (action)
        {
            case "getAllEvents":
                await SendAllEvents(webSocket);
                break;
            case "getEvent":
                if (request.TryGetValue("eventId", out var eventIdString) && Guid.TryParse(eventIdString, out var eventId))
                {
                    await SendEventById(webSocket, eventId);
                }
                break;
        }
    }

    private async Task SendAllEvents(WebSocket webSocket)
    {
        await SendMessage(webSocket, _esportService.GetAllEsportEvents());
    }

    private async Task SendEventById(WebSocket webSocket, Guid eventId)
    {
        var eventDetails = _esportService.GetEsportEventById(eventId);
        await SendMessage(webSocket, eventDetails);
    }

    private static async Task SendMessage<T>(WebSocket webSocket, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

    // Метод для рассылки обновлений всем клиентам
    public static async Task BroadcastUpdate<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        foreach (var socket in _sockets.Keys)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
        }
    }
}