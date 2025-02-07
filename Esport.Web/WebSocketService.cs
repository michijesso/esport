namespace Esport.Web;

using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using System.Text.Json;
using Domain;
using Domain.Models;

public class WebSocketService : IWebSocketService
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _allEventsSubscribers = new();
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, WebSocket>> _specificEventSubscribers = new();

    private readonly IServiceProvider _serviceProvider;

    public WebSocketService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void AddSocketForAllEvents(Guid connectionId, WebSocket socket)
    {
        _allEventsSubscribers[connectionId] = socket;
    }
    
    public void AddSocketForSpecifiedEvent(Guid connectionId, WebSocket socket, Guid eventId)
    {
        if (!_specificEventSubscribers.ContainsKey(eventId))
        {
            _specificEventSubscribers[eventId] = new ConcurrentDictionary<Guid, WebSocket>();
        }

        _specificEventSubscribers[eventId][connectionId] = socket;
    }

    public async Task RemoveSocketForAllEventsAsync(Guid connectionId)
    {
        if (_allEventsSubscribers.TryRemove(connectionId, out var socket))
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            socket.Dispose();
        }
    }
    
    public async Task RemoveSocketForSpecificEventAsync(Guid connectionId, Guid eventId)
    {
        if (_specificEventSubscribers.ContainsKey(eventId))
        {
            if (_specificEventSubscribers[eventId].TryRemove(connectionId, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                socket.Dispose();
            }
        }
    }
    
    public async Task BroadcastMessageAsync(string message, Guid? eventId = null)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var tasks = new List<Task>();

        foreach (var socket in _allEventsSubscribers.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                tasks.Add(socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
            }
        }

        if (eventId.HasValue && _specificEventSubscribers.ContainsKey(eventId.Value))
        {
            var specificEventSubscribers = _specificEventSubscribers[eventId.Value];

            foreach (var socket in specificEventSubscribers.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    tasks.Add(socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
                }
            }
        }

        await Task.WhenAll(tasks);
    }
    
    public async Task HandleWebSocketForAllEventsAsync(WebSocket webSocket)
    {
        var connectionId = Guid.NewGuid();
        AddSocketForAllEvents(connectionId, webSocket);
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var esportRepository = scope.ServiceProvider.GetRequiredService<IEsportRepository<EsportEvent>>();
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var allEvents = await esportRepository.GetAllAsync();
                var response = JsonSerializer.Serialize(allEvents);

                if (!string.IsNullOrEmpty(response))
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            {
                await RemoveSocketForAllEventsAsync(connectionId);
            }
            Console.WriteLine("Connection closed.");
        }
    }

    public async Task HandleWebSocketForSpecifiedEventAsync(WebSocket webSocket, Guid eventId)
    {
        var connectionId = Guid.NewGuid();
        AddSocketForSpecifiedEvent(connectionId, webSocket, eventId);
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var esportRepository = scope.ServiceProvider.GetRequiredService<IEsportRepository<EsportEvent>>();
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                await esportRepository.GetByIdAsync(eventId);
                var response = $"Subscribed to event with ID {eventId}";
                if (!string.IsNullOrEmpty(response))
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            {
                await RemoveSocketForSpecificEventAsync(connectionId, eventId);
            }
            Console.WriteLine("Connection closed.");
        }
    }
}