namespace Esport.Web;

using System.Net.WebSockets;

public interface IWebSocketService
{
    void AddSocket(Guid connectionId, WebSocket webSocket);
    Task RemoveSocketAsync(string socketId);
    Task BroadcastMessageAsync(string message);
    //Task SendToAllWebSockets(string message);
    Task HandleWebSocketAsync(WebSocket webSocket);
}