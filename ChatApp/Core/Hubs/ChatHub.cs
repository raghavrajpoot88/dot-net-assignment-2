using ChatApp.Core.Hubs;
using System.Threading.Tasks;
using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Xml.Linq;

namespace ChatApp.Hubs
{
    //public class ChatHub :Hub
    //{
    //    public async Task NewMessage( Messages message)
    //    {
    //        await Clients.All.SendAsync("ReceiveMessage",message);
    //    }

    //    public string GetConnectionId() => Context.ConnectionId;



    //}

    //[Authorize]
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

        public override Task OnConnectedAsync()
        {
            //// Add a test claim directly to the ClaimsPrincipal for testing purposes.
            //var identity = new ClaimsIdentity(new Claim[]
            //{
            //    new Claim(ClaimTypes.NameIdentifier, "TestUserId"),
            //    // Add other claims if needed.
            //});

            //Context.User.AddIdentity(identity);
            //string name = Context.User.Identity.Name;
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

        //public override Task OnReconnected()
        //{
        //    string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (!_connections.GetConnections(userId).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(name, Context.ConnectionId);
        //    }

        //    return base.OnReconnected();
        //}
    }
}
