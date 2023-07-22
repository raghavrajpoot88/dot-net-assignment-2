using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
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
        private readonly IConfiguration _configuration;
        public UserController(IUser user, IConfiguration configuration)
        {
            _user = user;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.Email)?.Value;
            var Users = _user.GetUsers();
            return Ok(Users);
        }


        [HttpGet("{id}")]
        public IActionResult GetUser(Guid id)
        {
            var User = _user.GetUserById(id);
            return Ok(User);
        }


        [HttpPost("register")]
        public IActionResult PostUser(UserDTO registered)
        {
            _user.ValidateRegistration(registered);

            CreatePasswordHash(registered.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var registeredUser = new User();

            registeredUser.Email = registered.Email;
            registeredUser.Name = registered.Name;
            registeredUser.PasswordHash = passwordHash;
            registeredUser.PasswordSalt = passwordSalt;

            _user.AddUser(registeredUser);
           
            return Ok(registeredUser);
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDTO login)
        {

            var user = _user.ValidateLogin(login);
            if (user == null || user.Email != login.Email)
            {
                return BadRequest("User is not Registered");
            }
            if (!VerifyPass(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong Password.");
            }
            string token = CreateToken(user);
            
            return Ok(new { token });
        }
        private string CreateToken(User user)
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
        private bool VerifyPass(string password, byte[] passwordHash, byte[] passwordSalt)
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
