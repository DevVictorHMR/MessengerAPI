using Microsoft.AspNetCore.Mvc;
using MessengerAPI.DTOs;
using MessengerAPI.Services;

namespace MessengerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _messageService.GetAllMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var message = await _messageService.GetMessageByIdAsync(id);
            return Ok(message);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageDto messageDto)
        {
            var createdMessage = await _messageService.CreateMessageAsync(messageDto);
            return CreatedAtAction(nameof(GetById), new { id = createdMessage.Id }, createdMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _messageService.DeleteMessageAsync(id);
            return NoContent();
        }
    }
}