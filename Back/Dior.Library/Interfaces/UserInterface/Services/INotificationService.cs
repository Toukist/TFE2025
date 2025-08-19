using Dior.Library.DTO;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface INotificationService
    {
        // ===== MÉTHODES EXISTANTES (préservées) =====
        List<NotificationDto> GetByUserId(int userId);
        void MarkAsRead(int id);
        void Delete(int id);
        int Add(NotificationDto dto, string createdBy);
        void Add(NotificationDto notificationDto);

        // ===== NOUVELLES MÉTHODES ASYNC POUR ANGULAR =====
        Task<List<NotificationDto>> GetByUserIdAsync(long userId);
        Task<List<NotificationDto>> GetUnreadByUserIdAsync(long userId);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> MarkAllAsReadAsync(long userId);
        Task<NotificationDto> CreateAsync(CreateNotificationRequest request, string createdBy = "System");
        Task<bool> SendBulkNotificationsAsync(List<long> userIds, string message, string type, string createdBy = "System");
        Task<bool> DeleteAsync(int id);
    }
}