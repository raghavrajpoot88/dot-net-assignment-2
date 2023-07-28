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
using Newtonsoft.Json.Linq;
using ChatApp.Core.Hubs;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;
using System;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageInfoController : ControllerBase
    {
        private readonly IMessages _messageInfo;
        private readonly IConfiguration _configuration;
        private readonly IMessagesService _messagesService;
        //private readonly IHubContext<ChatHub> _hubContext;
        //,IHubContext<ChatHub ,IChatHub> hubContext 

        public MessageInfoController(IMessages messageInfo, IConfiguration configuration,
                                        IMessagesService messagesService)
        {
            _messageInfo = messageInfo;
            _configuration = configuration;
            _messagesService = messagesService;
            //_hubContext = hubContext;
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

        [HttpPost("send")]
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
                    MsgId = Guid.NewGuid(),
                    Id = senderId.Id,
                    ReceiverId = messageInfo.ReceiverId,
                    MsgBody = messageInfo.MsgBody,
                    TimeStamp = DateTime.UtcNow
                };
                _messagesService.AddMessageService(message);
                //Broadcast the message from this point
                //await _hubContext.Clients.Client(message.ReceiverId).NewMessage( message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //private string GetConnectionId()
        //{
            
        //        this._hubConnection.invoke('GetConnectionId')
        //        .then((data) => {
        //            console.log(data);
        //            this.connectionId = data;
        //        });
            
        //}

        //    private getConnectionId = () => {
        //  this._hubConnection.invoke('GetConnectionId')
        //  .then((data) => {
        //        console.log(data);
        //        this.connectionId = data;
        //    });
        //}


    [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Messages>> UpdateMessage(Guid id, updateDTO Msg)
        {
            try
            {
                bool isEditedMessages = false;
                var result = "Message edited successfully";
                var currentUser = GetSenderIdFromToken();
                var SenderId = await _messagesService.GetLoggedUser(currentUser);
                var Message = await _messagesService.GetMessageId(id);

                if (Message.Id != SenderId.Id) return Unauthorized("Unauthorized access");
                if (Message == null || id != Message.MsgId) return NotFound($"Message not found ");

                Message.MsgBody = Msg.content;
                
                var UpdatedMsg = await _messagesService.UpdateMessageService(Message);
                if (UpdatedMsg != null)
                {
                    isEditedMessages = true;
                    return Ok(new { isEditedMessages, result });
                }
                return UpdatedMsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveMsg(Guid id)
        {
            try
            {
                bool isDeletdMessages = false;
                var result = "Message deleted successfully";
                var currentUser = GetSenderIdFromToken();
                var SenderId = await _messagesService.GetLoggedUser(currentUser);
                
                Messages User = await _messagesService.GetMessageId(id);

                if (User.Id != SenderId.Id) return Unauthorized("Unauthorized access");
                if (User == null || id != User.MsgId) return NotFound($"Message not found ");

                var response = await _messagesService.Delete(id);
                if(response)return Ok(new { isDeletdMessages,result });

                return NotFound($"Message not found ");
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
