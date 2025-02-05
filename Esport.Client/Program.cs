using System.Net.WebSockets;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        using (var client = new ClientWebSocket())
        {
            try
            {
                await client.ConnectAsync(new Uri("ws://localhost:5088/ws"), CancellationToken.None);
                Console.WriteLine("Connected to the WebSocket server");

                var receiveTask = Task.Run(async () =>
                {
                    var buffer = new byte[1024 * 4];
                    while (client.State == WebSocketState.Open)
                    {
                        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received: {message}");
                    }
                });

                while (client.State == WebSocketState.Open)
                {
                    Console.WriteLine("Enter a command (GetAllEvents or GetEventById {id}):");
                    var command = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(command))
                        continue;

                    var bytes = Encoding.UTF8.GetBytes(command);
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                await receiveTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}