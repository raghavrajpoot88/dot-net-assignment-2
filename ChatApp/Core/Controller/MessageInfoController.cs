﻿using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ChatApp.MiddleLayer.Services;

namespace ChatApp.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageInfoController : ControllerBase
    {
        private readonly IMessagesService _messagesService;
        public MessageInfoController( IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        // Get Conversation history of messages
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConversationHistory(string UserId, DateTime? before = null, int count = 20, string sort = "asc")
        {
            string currentUser = GetSenderIdFromToken();
            var result = await _messagesService.coversationHistory(UserId, currentUser, before,count,sort);
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
                return Ok(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


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
                if (User == null || id != User.MsgId) return NotFound("Message not found ");

                var response = await _messagesService.Delete(id);
                if (response) 
                {
                    isDeletdMessages = true;
                    return Ok(isDeletdMessages);
                }

                return NotFound(isDeletdMessages);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchConversations([FromQuery] string query)
        { 
            string currentUser = GetSenderIdFromToken();
            var SenderId = await _messagesService.GetLoggedUser(currentUser);
            if (string.IsNullOrEmpty(currentUser)) return Unauthorized();
            try
            {
                // Use the repository to search for messages
                var messages = _messagesService.Search(SenderId.Id, query);
                if(messages == null) return BadRequest("Invalid request parameters");
                return Ok(messages);
            }
            catch (Exception ex)
            {
                throw;
                //new { error = ex.Message }
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
                throw new Exception("Invalid token.", ex);      
            }
        }
    }
}
