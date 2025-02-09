namespace Esport.Web.Implementations;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Domain;
using Dtos;
using Interfaces;

public class WebSocketAllEventService : IWebSocketAllEventsService
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _allEventsSubscribers = new();
    
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<WebSocketAllEventService> _logger;

    public WebSocketAllEventService(IServiceProvider serviceProvider, IMapper mapper, ILogger<WebSocketAllEventService> logger)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _logger = logger;
    }
    
    public void AddSocketForAllEvents(Guid connectionId, WebSocket socket)
    {
        _allEventsSubscribers[connectionId] = socket;
    }
    
    public async Task RemoveSocketForAllEventsAsync(Guid connectionId)
    {
        if (_allEventsSubscribers.TryRemove(connectionId, out var socket))
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            socket.Dispose();
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
                await RemoveSocketForAllEventsAsync(connectionId);
            }
            _logger.LogInformation("Connection closed.");
        }
    }
}