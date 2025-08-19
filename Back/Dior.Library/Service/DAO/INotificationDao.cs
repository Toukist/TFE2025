using Dior.Library.DTO;

namespace Dior.Library.Service.DAO
{
    public interface INotificationDao
    {
        List<NotificationDto> GetByUserId(int userId);
        void MarkAsRead(int id);
        void Delete(int id);
        int Add(NotificationDto dto, string createdBy);
    }
}