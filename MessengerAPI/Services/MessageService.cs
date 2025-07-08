using MessengerAPI.Data;
using MessengerAPI.DTOs;
using MessengerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
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
            return await _context.Messages
                .Where(m => m.Id == id)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId
                }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Message not found");
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
    public class SqsConsumerService : BackgroundService
    {
        private readonly ISqsService _sqsService;
        private readonly IConfiguration _config;

        public SqsConsumerService(ISqsService sqsService, IConfiguration config)
        {
            _sqsService = sqsService;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueUrl = _config["AWS:QueueUrl"];

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _sqsService.ReceiveMessageFromQueueAsync(queueUrl);
                if (message != null)
                {
                    Console.WriteLine($"Mensagem recebida: {message}");
                    // Aqui você pode processar a mensagem, salvar, notificar, etc.
                }

                await Task.Delay(5000, stoppingToken); // A cada 5 segundos
            }
        }
    }
}