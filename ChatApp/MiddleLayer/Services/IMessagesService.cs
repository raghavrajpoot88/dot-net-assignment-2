using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IMessagesService
    {
        void AddMessageService(Messages messages);
        Task<ICollection<Messages>> coversationHistory(string UserId, string email, DateTime? before);
        Task<bool> Delete(Guid id);
        Task<IdentityUser> GetLoggedUser(string email);
        Task<Messages> GetMessageId(Guid id);
        Task<Messages> UpdateMessageService(Messages messageInfo);
    }
}
