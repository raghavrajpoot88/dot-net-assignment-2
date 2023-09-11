using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IMessages
    {
        public Task<ICollection<Messages>> GetMessage();
        Task<Messages> GetMessageById(Guid id);
        public Task<ICollection<Messages>> GetUser(string id);
        public Task<ICollection<Messages>> GetConversationHistory(string UserId, string email, DateTime? before, int count = 20, string sort = "asc");
        public Task AddMessage(Messages messageInfo);
        public Task<bool> RemoveMessage(Guid MsgId);
        public Task<Messages> UpdateMessage(Messages messageInfo);

        public Task<IdentityUser> GetCurrentUser(string email);
        public IEnumerable<Messages> SearchMessages(string userId, string query);
    }
}
