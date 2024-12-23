using Microsoft.AspNetCore.SignalR;

namespace WhiteboardServer.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("new client");
            await Clients.All.SendAsync("ReceiveMessage", $"Received the message: Another connection has been added.");
        }

        public async Task SendMessage(string message)
        {
            Console.WriteLine("New message: " + message);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", $"Received the message: {message}");
        }
    }
}
