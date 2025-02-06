namespace Esport.Web;

using System.Net.WebSockets;
using Dtos;

public interface IWebSocketService
{
    void AddSocket(Guid connectionId, WebSocket webSocket);
    Task RemoveSocketAsync(Guid socketId);
    Task BroadcastMessageAsync(string message);
    Task HandleWebSocketAsync(WebSocket webSocket);
}