using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Results;

namespace ChatApp.MiddleLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUser _userRepo;
        private readonly IConfiguration _configuration;

        public UserService(IUser userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        public async Task<ICollection<IdentityUser>> GetUsers()
        {
            var result = await _userRepo.GetUsers();
            return result; 
        }

        public async Task<RegistrationPara> GetUserById(string id)
        {
            var result = await _userRepo.GetUserById(id);
            var response = new RegistrationPara
            {
                UserId = result.Id,
                Name = result.UserName,
                Email = result.Email,
            };
            return response;
        }
        public async Task<RegistrationPara> ValidateRegistration(UserDTO u)
        {
            try
            {
                //Validations
                if (string.IsNullOrEmpty(u.Email))
                    throw new Exception("Email Required");
                if (string.IsNullOrEmpty(u.Name))
                    throw new Exception("User name Required");
                //if (string.IsNullOrEmpty(registered.Password) || registered.Password == "string")
                //    throw new Exception("Password Required");
                if (!await _userRepo.isUserExist(u.Email))
                    //return StatusCodes.Status409Conflict;
                    throw new Exception (" Registration failed because the email is already registered");

                var registeredUser = new IdentityUser {
                    Email = u.Email,
                    UserName = u.Name
                };
                
                await _userRepo.AddUser(registeredUser, u.Password);
                
                var response = new RegistrationPara
                { 
                    UserId = registeredUser.Id,
                    Name = registeredUser.UserName,
                    Email = registeredUser.Email,
                };
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        public async Task<IdentityUser> CheckUserRegister(loginDTO login)
        {   
            var result = await _userRepo.checkUser(login);
            return result;
        }

        public async Task<IdentityUser> AuthenticateGoogleUser(googleLoginDTO googleLogin)
        {
            var result = await _userRepo.LoginGoogleUser(googleLogin);
            return result;
        }

        public string GenerateToken(IdentityUser login)
        {
            string token =  CreateToken(login);
            return token;
        }

        private string CreateToken(IdentityUser user)
        {
            List<Claim> claims = new List<Claim>();
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
