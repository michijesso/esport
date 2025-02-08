namespace Esport.Web;

using System.Net.WebSockets;

public interface IWebSocketService
{
    void AddSocketForAllEvents(Guid connectionId, WebSocket socket);
    void AddSocketForSpecifiedEvent(Guid connectionId, WebSocket socket, int eventId);
    Task RemoveSocketForAllEventsAsync(Guid connectionId);
    Task RemoveSocketForSpecificEventAsync(Guid connectionId, int eventId);
    Task BroadcastSpecifiedEventAsync(string message, int eventId);
    Task BroadcastAllEventsAsync(string message);
    Task HandleWebSocketForAllEventsAsync(WebSocket webSocket);
    Task HandleWebSocketForSpecifiedEventAsync(WebSocket webSocket, int eventId);
}