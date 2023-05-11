
using messaging_app_API.Dto;
using messaging_app_API.Dtos;
using messaging_app_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace messaging_app_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }
        [HttpPost("register-user")]
        public IActionResult RegisterUser(ChatDto model)
        {
            if (_chatService.AddUserToList(model.Name))
            {
                // 204 status code
                return NoContent();
            }
            return BadRequest("This name is taken please choose another name");
        }
    }
}
