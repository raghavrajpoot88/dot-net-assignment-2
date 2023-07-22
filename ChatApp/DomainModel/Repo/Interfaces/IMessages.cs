using ChatApp.DomainModel.Models;
using Org.BouncyCastle.Bcpg.Sig;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IMessages
    {
        public Task<ICollection<Messages>> GetMessage();
        Task<Messages> GetMessageById(Guid id);
        public Task<ICollection<Messages>> GetUser(Guid id);
        public Task<ICollection<Messages>> GetConversationHistory(Guid UserId, string email, DateTime? before);
        public Task AddMessage(Messages messageInfo);
        public Task RemoveMessage(Guid MsgId);
        public Task<Messages> UpdateMessage(Messages messageInfo);

        public User GetCurrentUser(string email);
    }
}
