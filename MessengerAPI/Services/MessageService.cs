using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MessengerAPI.Data;
using MessengerAPI.DTOs;
using MessengerAPI.Models;
using MessengerAPI.Services;

namespace MessengerAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRabbitMqService _rabbitMqService;

        public MessageService(ApplicationDbContext context, IRabbitMqService rabbitMqService)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<List<MessageDto>> GetAllMessagesAsync()
        {
            return await _context.Messages
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId
                }).ToListAsync();
        }

        public async Task<MessageDto> GetMessageByIdAsync(int id)
        {
            var message = await _context.Messages
                .Where(m => m.Id == id)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId
                })
                .FirstOrDefaultAsync();

            if (message == null)
            {
                throw new KeyNotFoundException("Mensagem não encontrada.");
            }

            return message;
        }

        public async Task<MessageDto> CreateMessageAsync(MessageDto messageDto)
        {
            var message = new Message
            {
                Content = messageDto.Content,
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            messageDto.Id = message.Id;

            var messageJson = JsonConvert.SerializeObject(new
            {
                messageDto.Id,
                messageDto.Content,
                messageDto.SenderId,
                messageDto.ReceiverId,
                messageDto.Timestamp
            });

            _rabbitMqService.PublishMessage(messageJson);

            return messageDto;
        }

        public async Task DeleteMessageAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}