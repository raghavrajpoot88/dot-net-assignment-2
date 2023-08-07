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

        public async Task<Messages> GetMessageId(Guid id)
        {
            return await _messages.GetMessageById(id);
        }


        public async Task<ICollection<Messages>> coversationHistory(string UserId, string email, DateTime? before, int count = 20, string sort = "asc")
        {
            return await _messages.GetConversationHistory(UserId, email, before,count,sort); 
        }

        public async Task<IdentityUser> GetLoggedUser(string email)
        {
            return await _messages.GetCurrentUser(email) ;
        }

        public void AddMessageService(Messages messages)
        {
            _messages.AddMessage(messages);
        }

        public async Task<Messages> UpdateMessageService(Messages messageInfo)
        {
            var result = await _messages.UpdateMessage(messageInfo);
            return result;
        }

        public async Task<bool> Delete(Guid id)
        {
            var result = await _messages.RemoveMessage(id);
            return result;
        }
        public IEnumerable<Messages> Search(string userId, string query)
        {
            var result =_messages.SearchMessages(userId,query); 
            return result;
        }
    }
}
