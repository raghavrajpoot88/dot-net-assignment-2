using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IUserService
    {
        Task<RegistrationPara> CheckUserRegister(loginDTO login);
        string GenerateToken(loginDTO login);
        Task<RegistrationPara> GetUserById(string id);
        Task<RegistrationPara> ValidateRegistration(UserDTO u);
        bool VerifyPass(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}