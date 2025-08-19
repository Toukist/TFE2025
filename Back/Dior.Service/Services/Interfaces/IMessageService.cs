using Dior.Library.DTO;

namespace Dior.Service.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto> SendToTeamAsync(long senderId, CreateMessageRequest request);
        Task<MessageDto> SendToUserAsync(long senderId, CreateMessageRequest request);
        Task<List<MessageDto>> GetUserMessagesAsync(long userId);
        Task<List<MessageDto>> GetTeamMessagesAsync(int teamId);
        Task<bool> MarkAsReadAsync(long messageId);
        Task<bool> DeleteAsync(long messageId);
        Task<int> GetUnreadCountAsync(long userId);
    }
}