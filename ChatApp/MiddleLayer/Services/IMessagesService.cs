using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IMessagesService
    {
        void AddMessageService(Messages messages);
        Task<ICollection<Messages>> coversationHistory(string UserId, string email, DateTime? before, int count = 20, string sort = "asc");
        Task<bool> Delete(Guid id);
        Task<IdentityUser> GetLoggedUser(string email);
        Task<Messages> GetMessageId(Guid id);
        IEnumerable<Messages> Search(string userId, string query);
        Task<Messages> UpdateMessageService(Messages messageInfo);
    }
}
