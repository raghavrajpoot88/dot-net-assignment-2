﻿using ChatApp.DomainModel.Models;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg.Sig;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IMessages
    {
        public Task<ICollection<Messages>> GetMessage();
        Task<Messages> GetMessageById(string id);
        public Task<ICollection<Messages>> GetUser(string id);
        public Task<ICollection<Messages>> GetConversationHistory(string UserId, string email, DateTime? before);
        public Task AddMessage(Messages messageInfo);
        public Task RemoveMessage(string MsgId);
        public Task<Messages> UpdateMessage(Messages messageInfo);

        public Task<IdentityUser> GetCurrentUser(string email);
    }
}
