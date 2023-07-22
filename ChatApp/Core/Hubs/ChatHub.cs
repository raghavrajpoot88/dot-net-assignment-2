using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub :Hub
    {
        public async Task NewMessage(long username, string message) =>
        await Clients.All.SendAsync("messageReceived", username, message);
    }
}
