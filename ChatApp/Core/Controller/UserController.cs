﻿using ChatApp.Hubs;
using ChatApp.MiddleLayer.DTOs;
using ChatApp.MiddleLayer.ResponseParameter;
using ChatApp.MiddleLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _hubContext;

        public UserController(IUserService userService, IHubContext<ChatHub> hubContext)
        {
            _userService = userService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var Users = await _userService.GetUsers();
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
            var response = new RegistrationPara
            {
                Id = result.Id,
                Name = result.UserName,
                Email = result.Email,
            };

            if (result != null)
            {
                string token = _userService.GenerateToken(result);
                return Ok(new { token, response });
            }
            return BadRequest("User is not Found");
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuthenticate([FromBody] googleLoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(it => it.Errors).Select(it => it.ErrorMessage));
            }
            var token = _userService.GenerateToken(await _userService.AuthenticateGoogleUser(request));

            return Ok((new { token }));
        }
    }
}

