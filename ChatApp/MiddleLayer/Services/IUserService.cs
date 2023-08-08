using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.MiddleLayer.Services
{
    public interface IUserService
    {
        Task<ICollection<RegistrationPara>> GetUsers();
        Task<RegistrationPara> GetUserById(string id);
        Task<IdentityUser> AuthenticateGoogleUser(googleLoginDTO googleLogin);
        Task<IdentityUser> CheckUserRegister(loginDTO login);
        string GenerateToken(IdentityUser login);
        Task<RegistrationPara> ValidateRegistration(UserDTO u);
       
    }
}