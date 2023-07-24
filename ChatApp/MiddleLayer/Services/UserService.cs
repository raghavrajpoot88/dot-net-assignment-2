using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
                if (string.IsNullOrEmpty(u.Email) || u.Email == "string")
                    throw new Exception("Email Required");
                if (string.IsNullOrEmpty(u.Name) || u.Name == "string")
                    throw new Exception("User name Required");
                //if (string.IsNullOrEmpty(registered.Password) || registered.Password == "string")
                //    throw new Exception("Password Required");
                //var CheckUserExists = (from user in _user.GetUsers()
                //                       where user.Email.Equals(u.Email)
                //                       select user).Count();
                //if (CheckUserExists > 0)
                //{
                //    throw new Exception("User Already exist");
                //}
                //CreatePasswordHash(u.Password, out byte[] passwordHash, out byte[] passwordSalt);
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
        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        //    }
        //}

        public async Task<RegistrationPara> CheckUserRegister(loginDTO login)
        {   
            var result = await _userRepo.checkUser(login);
            if (result != null)
            {
                var response = new RegistrationPara
                {
                    UserId = result.Id,
                    Name = result.UserName,
                    Email = result.Email,
                };
                return response;
            }
            return null;
        }

        public string GenerateToken(loginDTO login)
        {
            string token = CreateToken(login);
            return token;
        }

        private string CreateToken(loginDTO user)
        {
            List<Claim> claims = new List<Claim>();
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
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
        public bool VerifyPass(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (password == null || passwordHash == null || passwordSalt == null)
            {
                return false; // Handle the case where any of the parameters are null
            }
            try
            {
                using (var hmac = new HMACSHA512(passwordSalt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return computedHash.SequenceEqual(passwordHash);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
