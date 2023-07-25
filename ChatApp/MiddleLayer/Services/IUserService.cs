using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IUserService
    {
        Task<RegistrationPara> CheckUserRegister(loginDTO login);
        Task<string> GenerateToken(loginDTO login);
        Task<RegistrationPara> GetUserById(string id);
        Task<ICollection<IdentityUser>> GetUsers();
        Task<RegistrationPara> ValidateRegistration(UserDTO u);
        bool VerifyPass(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}