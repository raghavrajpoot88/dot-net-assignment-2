using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using ChatApp.MiddleLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IUserService _userService;

        public UserController(IUser user,IUserService userService)
        {
            _user = user;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.Email)?.Value;
            var Users = _user.GetUsers();
            return Ok(Users);
        }


        [HttpGet("{id}")]
        public IActionResult GetUser(string id)
        {
            _userService.GetUserById(id);
            return Ok(User);
        }


        [HttpPost("register")]
        public async Task<IActionResult> PostUser(UserDTO registered)
        {
            var result = await _userService.ValidateRegistration(registered);
            return Ok(result);
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDTO login)
        {
            var result = await _userService.CheckUserRegister(login);
           
            if (result!=null)
            {
                string token = _userService.GenerateToken(login);
                return Ok(new { token,result });
                
            }
                return BadRequest("User is not Found");
            //if (!_userService.VerifyPass(login.Password, user.PasswordHash, user.PasswordSalt))
            //{
            //    return BadRequest("Wrong Password.");
            //}
             
            
        }
    }
}
