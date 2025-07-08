using MessengerAPI.DTOs;

namespace MessengerAPI.Services
{
    public interface IMessageService
    {
        Task<List<MessageDto>> GetAllMessagesAsync();
        Task<MessageDto> GetMessageByIdAsync(int id);
        Task<MessageDto> CreateMessageAsync(MessageDto messageDto);
        Task DeleteMessageAsync(int id);
    }
}