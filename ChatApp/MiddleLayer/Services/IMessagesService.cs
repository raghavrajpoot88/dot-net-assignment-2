using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IMessagesService
    {
        Task<ICollection<Messages>> coversationHistory(string UserId, string email, DateTime? before);
        Task<IdentityUser> GetLoggedUser(string email);
        Task<Messages> GetMessageId(string id);
    }
}
