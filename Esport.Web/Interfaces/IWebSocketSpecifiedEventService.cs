namespace Esport.Web.Interfaces;

using System.Net.WebSockets;

public interface IWebSocketSpecifiedEventService
{
    void AddSocketForSpecifiedEvent(Guid connectionId, WebSocket socket, int eventId);
    Task RemoveSocketForSpecifiedEventAsync(Guid connectionId, int eventId);
    Task BroadcastSpecifiedEventAsync(string message, int eventId);
    Task HandleWebSocketForSpecifiedEventAsync(WebSocket webSocket, int eventId);
}