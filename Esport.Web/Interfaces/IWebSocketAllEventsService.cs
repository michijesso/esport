namespace Esport.Web.Interfaces;

using System.Net.WebSockets;

public interface IWebSocketAllEventsService
{
    void AddSocketForAllEvents(Guid connectionId, WebSocket socket);
    
    Task RemoveSocketForAllEventsAsync(Guid connectionId);
    
    Task BroadcastAllEventsAsync(string message);
    
    Task HandleWebSocketForAllEventsAsync(WebSocket webSocket);
}