using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IUser
    {
        Task<ICollection<IdentityUser>> GetUsers();
        public Task<IdentityUser> GetUserById(string id);
        public Task AddUser(IdentityUser registeredUser,string Password);
        public Task<IdentityUser> checkUser(loginDTO login);
        Task<List<Claim>> GetClaims(string email);
    }
}
