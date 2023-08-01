using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Google.Apis.Auth.GoogleJsonWebSignature;


using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.DomainModel.Repo
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserRepository(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _configuration = configuration;
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

        public async Task<IdentityUser> LoginGoogleUser(googleLoginDTO googleInfo)
        {
            Payload payload = await ValidateAsync(googleInfo.IdToken, new ValidationSettings
            {
                Audience = new[] { _configuration["Google:ClientId"] }
            });

           return await GetOrCreateExternalLoginUser(googleLoginDTO.PROVIDER, payload.Subject, payload.Email, payload.GivenName);
        }

        // ...
        private async Task<IdentityUser> GetOrCreateExternalLoginUser(string provider, string key, string email, string firstName)
        {
            var user = await _userManager.FindByLoginAsync(provider, key);
            if (user != null)
                return user;

            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    //UserName = email,
                    UserName = firstName,
                    //LastName = lastName,
                    Id = key,
                };
                await _userManager.CreateAsync(user);
            }

            var info = new UserLoginInfo(provider, key, provider.ToUpperInvariant());
            var result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
                return user;

            return null;
        }




    }
}
