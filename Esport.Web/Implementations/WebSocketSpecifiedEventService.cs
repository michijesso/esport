namespace Esport.Web.Implementations;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Domain;
using Dtos;
using Interfaces;

public class WebSocketSpecifiedEventService : IWebSocketSpecifiedEventService
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, WebSocket>> _specificEventSubscribers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<WebSocketSpecifiedEventService> _logger;

    public WebSocketSpecifiedEventService(IServiceProvider serviceProvider, IMapper mapper, ILogger<WebSocketSpecifiedEventService> logger)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _logger = logger;
    }
    
        public void AddSocketForSpecifiedEvent(Guid connectionId, WebSocket socket, int eventId)
    {
        if (!_specificEventSubscribers.ContainsKey(eventId))
        {
            _specificEventSubscribers[eventId] = new ConcurrentDictionary<Guid, WebSocket>();
        }

        _specificEventSubscribers[eventId][connectionId] = socket;
    }
    
    public async Task RemoveSocketForSpecifiedEventAsync(Guid connectionId, int eventId)
    {
        if (_specificEventSubscribers.TryGetValue(eventId, out var subscriber))
        {
            if (subscriber.TryRemove(connectionId, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                socket.Dispose();
            }
        }
    }
    
    public async Task BroadcastSpecifiedEventAsync(string message, int eventId)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var tasks = new List<Task>();
        
        if (_specificEventSubscribers.TryGetValue(eventId, out var specificEventSubscribers))
        {
            tasks.AddRange(from socket in specificEventSubscribers.Values where socket.State == WebSocketState.Open select socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
        }

        await Task.WhenAll(tasks);
    }
    
    public async Task HandleWebSocketForSpecifiedEventAsync(WebSocket webSocket, int eventId)
    {
        var connectionId = Guid.NewGuid();
        AddSocketForSpecifiedEvent(connectionId, webSocket, eventId);
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var esportRepository = scope.ServiceProvider.GetRequiredService<IEsportRepository>();
            var buffer = new byte[1024 * 4];
            var esportEvent = esportRepository.GetByIdAsync(eventId);
            var mappedEvent = _mapper.Map<EsportEventDto>(esportEvent);
            var response = JsonSerializer.Serialize(mappedEvent);

            if (!string.IsNullOrEmpty(response))
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            
            //при комплите таски (любой ввод со стороны клиента), закрываем соединение
            await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError($"WebSocket exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            {
                await RemoveSocketForSpecifiedEventAsync(connectionId, eventId);
            }
            _logger.LogInformation("Connection closed.");
        }
    }
}