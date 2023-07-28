using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IUserService
    {
        Task<IdentityUser> CheckUserRegister(loginDTO login);
        string GenerateToken(IdentityUser login);
        Task<RegistrationPara> GetUserById(string id);
        Task<ICollection<IdentityUser>> GetUsers();
        Task<RegistrationPara> ValidateRegistration(UserDTO u);
       
    }
}