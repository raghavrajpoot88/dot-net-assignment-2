using ChatApp.Core.Hubs;
using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub :Hub
    {
        public async Task NewMessage( Messages message)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage",message);
        }

        //public string GetConnectionId() => Context.ConnectionId;



    }
}
