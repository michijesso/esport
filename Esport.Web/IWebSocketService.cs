namespace Esport.Web;

using System.Net.WebSockets;

public interface IWebSocketService
{
    void AddSocketForAllEvents(Guid connectionId, WebSocket socket);
    void AddSocketForSpecifiedEvent(Guid connectionId, WebSocket socket, Guid eventId);
    Task RemoveSocketForAllEventsAsync(Guid connectionId);
    Task RemoveSocketForSpecificEventAsync(Guid connectionId, Guid eventId);
    Task BroadcastMessageAsync(string message, Guid? eventId);
    Task HandleWebSocketForAllEventsAsync(WebSocket webSocket);
    Task HandleWebSocketForSpecifiedEventAsync(WebSocket webSocket, Guid eventId);
}