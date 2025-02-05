using System.Collections.Concurrent;
using System.Text.Json;
using Esport.Domain;
using Esport.Domain.Models;

namespace Esport.Web;

using System.Net.WebSockets;
using System.Text;

// public class WebSocketService : IWebSocketService
// {
//     private static readonly List<WebSocket> _webSocketList = new List<WebSocket>();
//     private readonly IServiceProvider _serviceProvider;
//
//     public WebSocketService(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//         
//     public void AddWebSocket(WebSocket webSocket)
//     {
//         _webSocketList.Add(webSocket);
//         Console.WriteLine($"WebSocket connection added. Total connections: {_webSocketList.Count}");
//     }
//
//     public async Task SendToAllWebSockets(string message)
//     {
//         var tasks = _webSocketList.Where(ws => ws.State == WebSocketState.Open)
//                                   .Select(ws => SendMessageAsync(ws, message)).ToList();
//
//         // Логирование отправки сообщений
//         Console.WriteLine($"Sending message to all WebSockets: {message}");
//
//         await Task.WhenAll(tasks);
//     }
//
//     private async Task SendMessageAsync(WebSocket webSocket, string message)
//     {
//         var buffer = Encoding.UTF8.GetBytes(message);
//         var segment = new ArraySegment<byte>(buffer);
//         await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
//     }
//
//     public async Task HandleWebSocketAsync(WebSocket webSocket)
//     {
//         var buffer = new byte[1024 * 4];
//         try
//         {
//             using (var scope = _serviceProvider.CreateScope())
//             {
//                 var esportRepository = scope.ServiceProvider.GetRequiredService<IEsportRepository<EsportEvent>>();
//                 WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//
//                 while (!result.CloseStatus.HasValue)
//                 {
//                     result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//
//                     var command = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                     string response = string.Empty;
//
//                     if (command.Equals("GetAllEvents", StringComparison.OrdinalIgnoreCase))
//                     {
//                         var allEvents = await esportRepository.GetAllAsync();
//                         response = JsonSerializer.Serialize(allEvents);
//                     }
//                     else if (command.StartsWith("GetEventById", StringComparison.OrdinalIgnoreCase))
//                     {
//                         var parts = command.Split(' ');
//                         if (parts.Length == 2 && Guid.TryParse(parts[1], out var id))
//                         {
//                             var evt = await esportRepository.GetByIdAsync(id);
//                             response = evt != null ? JsonSerializer.Serialize(evt) : "Event not found";
//                         }
//                     }
//
//                     if (!string.IsNullOrEmpty(response))
//                     {
//                         var responseBytes = Encoding.UTF8.GetBytes(response);
//                         await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text,
//                             true, CancellationToken.None);
//                     }
//                 }
//             }
//         }
//         catch (WebSocketException ex)
//         {
//             Console.WriteLine($"WebSocket exception: {ex.Message}");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Exception: {ex.Message}");
//         }
//         finally
//         {
//             if (webSocket.State != WebSocketState.Closed)
//             {
//                 await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed connection", CancellationToken.None);
//             }
//             Console.WriteLine("Connection closed.");
//         }
//     }
// }



public class WebSocketService : IWebSocketService
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
    private readonly IServiceProvider _serviceProvider;

    public WebSocketService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void AddSocket(Guid connectionId, WebSocket socket)
    {
        _sockets[connectionId.ToString()] = socket;
    }

    public async Task RemoveSocketAsync(string connectionId)
    {
        if (_sockets.TryRemove(connectionId, out var socket))
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            socket.Dispose();
        }
    }

    public async Task BroadcastMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var tasks = new List<Task>();

        foreach (var socket in _sockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                tasks.Add(socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
            }
        }

        await Task.WhenAll(tasks);
    }
    
    public async Task HandleWebSocketAsync(WebSocket webSocket)
     {
         var buffer = new byte[1024 * 4];
         try
         {
             using (var scope = _serviceProvider.CreateScope())
             {
                 var esportRepository = scope.ServiceProvider.GetRequiredService<IEsportRepository<EsportEvent>>();
                 WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                 while (!result.CloseStatus.HasValue)
                 {
                     result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                     var command = Encoding.UTF8.GetString(buffer, 0, result.Count);
                     string response = string.Empty;

                     if (command.Equals("GetAllEvents", StringComparison.OrdinalIgnoreCase))
                     {
                         var allEvents = await esportRepository.GetAllAsync();
                         response = JsonSerializer.Serialize(allEvents);
                     }
                     else if (command.StartsWith("GetEventById", StringComparison.OrdinalIgnoreCase))
                     {
                         var parts = command.Split(' ');
                         if (parts.Length == 2 && Guid.TryParse(parts[1], out var id))
                         {
                             var evt = await esportRepository.GetByIdAsync(id);
                             response = evt != null ? JsonSerializer.Serialize(evt) : "Event not found";
                         }
                     }

                     if (!string.IsNullOrEmpty(response))
                     {
                         var responseBytes = Encoding.UTF8.GetBytes(response);
                         await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text,
                             true, CancellationToken.None);
                     }
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
             if (webSocket.State != WebSocketState.Closed)
             {
                 await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed connection", CancellationToken.None);
             }
             Console.WriteLine("Connection closed.");
         }
     }
}