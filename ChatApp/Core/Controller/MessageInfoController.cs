using ChatApp.DomainModel.Models;
using ChatApp.DomainModel;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatApp.MiddleLayer.Services;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageInfoController : ControllerBase
    {
        private readonly IMessages _messageInfo;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IMessagesService _messagesService;

        public MessageInfoController(IMessages messageInfo, IConfiguration configuration, ApplicationDbContext applicationDbContext,
            IMessagesService messagesService)
        {
            _messageInfo = messageInfo;
            _configuration = configuration;
            _context = applicationDbContext;
            _messagesService = messagesService;
        }

        // Get Conversation history of messages
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConversationHistory(string UserId, DateTime? before = null, int count = 20, string sort = "asc")
        {
            string currentUser = GetSenderIdFromToken();
            var result = await _messagesService.coversationHistory(UserId, currentUser, before);
            //var result = await _messageInfo.GetConversationHistory(UserId, currentUser, before);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Messages>> AddMessaage(MessagesDTO messageInfo)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                string currentUser = GetSenderIdFromToken();
                var senderId = await _messagesService.GetLoggedUser(currentUser);

                // Create a new message object
                var message = new Messages
                {
                    
                    UserId = senderId.Id,
                    ReceiverId = messageInfo.ReceiverId,
                    MsgBody = messageInfo.MsgBody,
                    TimeStamp = DateTime.UtcNow
                };
                await _messageInfo.AddMessage(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Messages>> UpdateMessage(string id, updateDTO Msg)
        {
            try
            {
                var currentUser = GetSenderIdFromToken();

                var SenderId = await _messagesService.GetLoggedUser(currentUser);
                var User = await _messagesService.GetMessageId(id);

                if (User.UserId != SenderId.Id) return Unauthorized();
                if (User == null || id != User.MsgId) return NotFound($"User Id={id} not found ");

                User.MsgBody = Msg.content;
                var UpdatedMsg = await _messageInfo.UpdateMessage(User);
                return UpdatedMsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveMsg(string id)
        {
            try
            {
                var currentUser = GetSenderIdFromToken();
                var SenderId = await _messagesService.GetLoggedUser(currentUser);
                Messages User = await _messageInfo.GetMessageById(id);

                if (User.UserId != SenderId.Id) return Unauthorized();
                if (User == null || id != User.MsgId) return NotFound($"User Id={id} not found ");

                await _messageInfo.RemoveMessage(id);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetSenderIdFromToken()
        {
            try
            {
                // Validate and decode the JWT token
                var claimsPrincipal = HttpContext.User;
                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
                if (emailClaim == null)
                {
                    throw new Exception("User Id not found in token.");
                }
                return emailClaim.Value;
            }
            catch (SecurityTokenException ex)
            {
                throw new Exception("Invalid token.", ex);      // Handle exceptions according to your application's needs
            }
        }

    }
}
