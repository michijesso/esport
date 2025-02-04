namespace Esport.Client;

using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class ClientSender
{
    public async Task SendAsync()
    {
        using var client = new ClientWebSocket();
        var uri = new Uri("ws://localhost:5088/api/ws/connect");
        await client.ConnectAsync(uri, CancellationToken.None);

        Console.WriteLine("Connected to WebSocket server.");
        
        // Отправка запроса на получение всех событий
        await SendMessage(client, new { action = "getAllEvents" });

        // Запуск фонового процесса чтения сообщений
        _ = Task.Run(() => ReceiveMessages(client));

        while (true)
        {
            Console.WriteLine("Enter event ID to fetch details (or type 'exit' to quit):");
            var input = Console.ReadLine();
            if (input?.ToLower() == "exit")
                break;

            await SendMessage(client, new { action = "getEvent", eventId = input });
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
        Console.WriteLine("WebSocket closed.");
    }

    static async Task SendMessage(ClientWebSocket client, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    static async Task ReceiveMessages(ClientWebSocket client)
    {
        var buffer = new byte[1024 * 4];

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Server closed connection.");
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine("Received: " + message);
        }
    }
}
