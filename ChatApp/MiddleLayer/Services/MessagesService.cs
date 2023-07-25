using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo;
using ChatApp.DomainModel.Repo.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public class MessagesService : IMessagesService 
    {
        private readonly IMessages _messages;

        public MessagesService( IMessages messages)
        {
            _messages = messages;
        }

        public async Task<Messages> GetMessageId(string id)
        {
            return await _messages.GetMessageById(id);
        }


        public async Task<ICollection<Messages>> coversationHistory(string UserId, string email, DateTime? before)
        {
            return await _messages.GetConversationHistory(UserId, email, before); 
        }

        public async Task<IdentityUser> GetLoggedUser(string email)
        {
            return await _messages.GetCurrentUser(email) ;
        }


    }
}
