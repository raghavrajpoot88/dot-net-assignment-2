using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using Google.Protobuf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DomainModel.Repo
{
    public class MessagesRepository : IMessages
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public MessagesRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }


        public async Task<ICollection<Messages>> GetMessage()
        {
            var result = await _applicationDbContext.messages.ToListAsync();
            return result;
        }
        public async Task<ICollection<Messages>> GetUser(string UserId)

        {

            var result = await _applicationDbContext.messages.
               Where(a => a.Id == UserId).ToListAsync();

            return result;
        }

        public async Task<Messages> GetMessageById(Guid id)
        {

            var result = await _applicationDbContext.messages.
               Where(a => a.MsgId == id).FirstOrDefaultAsync();

            return result;
        }
        public async Task<IdentityUser> GetCurrentUser(string email)
        {
            var senderId = await _userManager.FindByEmailAsync(email);
            return senderId;
        }

        public async Task<ICollection<Messages>> GetConversationHistory(string UserId, string currentUser, DateTime? before)
        {

            var senderId = await GetCurrentUser(currentUser);
            var MessageHistory = await _applicationDbContext.messages.Where (u => (u.ReceiverId == UserId && u.Id == senderId.Id)
                || (u.Id == UserId && u.ReceiverId == senderId.Id) && (before == null || u.TimeStamp < before))
                .OrderBy(m => m.TimeStamp).ToListAsync();

            return MessageHistory;
        }

        public async Task AddMessage(Messages messageInfo)
        {
            await _applicationDbContext.messages.AddAsync(messageInfo);
            await _applicationDbContext.SaveChangesAsync();

        }
        public async Task<Messages> UpdateMessage(Messages messageInfo)
        {
            var user = await _applicationDbContext.messages.FirstOrDefaultAsync(a => a.MsgId == messageInfo.MsgId);
            if (user != null)
            {
                user.MsgId = messageInfo.MsgId;
                user.Id= messageInfo.Id;
                user.ReceiverId = messageInfo.ReceiverId;
                user.MsgBody = messageInfo.MsgBody;
                user.TimeStamp = messageInfo.TimeStamp;

                await _applicationDbContext.SaveChangesAsync();
                return user;
            }
            return null;

        }
        public async Task<bool> RemoveMessage(Guid MsgId)
        {
            var result = await _applicationDbContext.messages.Where(a => a.MsgId == MsgId).FirstOrDefaultAsync();
            if (result != null)
            {
                _applicationDbContext.messages.Remove(result);
                await _applicationDbContext.SaveChangesAsync();
                return true;

            }
            return false;

        }
        public IEnumerable<Messages> SearchMessages(string userId, string query)
        {
            var normalizedQuery = query.ToLower();

            var matchedMessages = _applicationDbContext.messages
                .Where(m => (m.Id == userId || m.ReceiverId == userId)
                         && m.MsgBody.ToLower().Contains(normalizedQuery))
                .ToList();
            //// Assuming you have a way to get the messages based on the user's ID
            //var userMessages = _applicationDbContext.messages.Where(m => m.Id == userId || m.ReceiverId == userId);

            //// Perform the search using the provided query
            //var matchedMessages = userMessages.Where(m => m.MsgBody.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList(); 

            return matchedMessages;
        }

    }
}
