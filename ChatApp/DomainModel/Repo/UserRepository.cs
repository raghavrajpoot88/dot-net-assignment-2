using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.DomainModel.Repo
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }
        public async Task<ICollection<IdentityUser>> GetUsers()
        {
            var result = await _userManager.Users.ToListAsync();
            return result;
        }
        public async Task<IdentityUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task AddUser(IdentityUser registeredUser,string Password)
        {

            await _userManager.CreateAsync(registeredUser, Password);
            
        }

        public async Task<IdentityUser> checkUser(loginDTO login)
        {
            var result = await _userManager.FindByEmailAsync(login.Email);
            if (result!= null && await _userManager.CheckPasswordAsync(result, login.Password)) 
            {
                return result;
            }
            

            return null;
        }

        


       
       
    }
}
