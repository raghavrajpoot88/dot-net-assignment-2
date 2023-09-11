using ChatApp.Core.Hubs;
using System.Threading.Tasks;
using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Xml.Linq;

namespace ChatApp.Hubs
{
    

    
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public async Task NewMessage(Messages message)
        {
            // Get the recipient's name from the message (assuming the recipient name is in the 'to' property).
            string recipientID = message.ReceiverId;

            // Get the connection IDs associated with the recipient's name.
            var connectionIds = _connections.GetConnections(recipientID);

            // Send the message to each of the recipient's connections.
            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
        public async Task EditMessage(Messages message)
        {
            string recipientID = message.ReceiverId;
            var connectionIds = _connections.GetConnections(recipientID);

            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveEditMessage", message);
            }
        }

        public async Task DeleteMessage(Messages message)
        {

            string recipientID = message.ReceiverId;
            var connectionIds = _connections.GetConnections(recipientID);

            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveDeleteMessage", message);
            }
        }

        public override Task OnConnectedAsync()
        {
            
            string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _connections.Add(userId, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            //string name = Context.User.Identity.Name;
            string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _connections.Remove(userId, Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

       
    }
}
