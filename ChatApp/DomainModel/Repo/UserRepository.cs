﻿using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using System.Security.Claims;
using System.Net;
using System.Web.Http;
using ChatApp.MiddleLayer.ResponseParameter;

namespace ChatApp.DomainModel.Repo
{
    public class UserRepository : IUser
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserRepository(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<ICollection<RegistrationPara>> GetUsers()
        {
            var result = await _userManager.Users
                .Select(u => new RegistrationPara
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                })
                .ToListAsync();
            return result;
        }
        public async Task<IdentityUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public async Task<bool> isUserExist(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            if(result==null) return false;
            return true;
        }
        

        public async Task AddUser(IdentityUser registeredUser,string Password)
        {
            await _userManager.CreateAsync(registeredUser, Password);
            await _userManager.AddClaimAsync(registeredUser, new Claim(ClaimTypes.Email, registeredUser.Email));
        }

        public async Task<IdentityUser> checkUser(loginDTO login)
        {
            var result = await _userManager.FindByEmailAsync(login.Email);
            await _userManager.AddClaimAsync(result, new Claim(ClaimTypes.Email, result.Email));
            if (!await _userManager.CheckPasswordAsync(result, login.Password))
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Login failed due to incorrect credentials" };
                throw new HttpResponseException(msg);
            }
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

        private async Task<IdentityUser> GetOrCreateExternalLoginUser(string provider, string key, string email, string firstName)
        {
            firstName = firstName.Replace(" ", ""); 

            var user = await _userManager.FindByLoginAsync(provider, key);
            if (user != null) return user;

            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    UserName = firstName,
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
