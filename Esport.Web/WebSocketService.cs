using AutoMapper;
using Esport.Web.Dtos;

namespace Esport.Web;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Domain;

public class WebSocketService : IWebSocketService
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _allEventsSubscribers = new();
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, WebSocket>> _specificEventSubscribers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    public WebSocketService(IServiceProvider serviceProvider, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }

    public void AddSocketForAllEvents(Guid connectionId, WebSocket socket)
    {
        _allEventsSubscribers[connectionId] = socket;
    }
    
    public void AddSocketForSpecifiedEvent(Guid connectionId, WebSocket socket, int eventId)
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
    
    public async Task RemoveSocketForSpecificEventAsync(Guid connectionId, int eventId)
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
    
    public async Task BroadcastAllEventsAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var tasks = new List<Task>();

        if (!_allEventsSubscribers.IsEmpty)
        {
            tasks.AddRange(from socket in _allEventsSubscribers.Values where socket.State == WebSocketState.Open select socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
        }

        await Task.WhenAll(tasks);
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
    
    public async Task HandleWebSocketForAllEventsAsync(WebSocket webSocket)
    {
        var connectionId = Guid.NewGuid();
        AddSocketForAllEvents(connectionId, webSocket);
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var esportRepository = scope.ServiceProvider.GetRequiredService<IEsportRepository>();
            var buffer = new byte[1024 * 4];

            var allEvents = await esportRepository.GetAllAsync();
            var mappedEvents = _mapper.Map<IEnumerable<EsportEventDto>>(allEvents);
            var response = JsonSerializer.Serialize(mappedEvents);

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