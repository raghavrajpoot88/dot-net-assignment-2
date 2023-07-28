using ChatApp.DomainModel.Models;

namespace ChatApp.Core.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage( Messages message);
        //public Task NewMessage( Messages message);
        //public string GetConnectionId();
    }
}
