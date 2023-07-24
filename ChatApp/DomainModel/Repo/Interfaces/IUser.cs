using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IUser
    {
        public ICollection<User> GetUsers();
        public Task<IdentityUser> GetUserById(string id);
        public Task AddUser(IdentityUser registeredUser,string Password);
        public Task<IdentityUser> checkUser(loginDTO login);
    }
}
