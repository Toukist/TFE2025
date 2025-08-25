using Dior.Library.DTO;

namespace Dior.Library.Service.DAO
{
    public interface INotificationDao
    {
        List<NotificationDto> GetByUserId(long userId);
        void MarkAsRead(long id);
        void Delete(long id);
        long Add(NotificationDto dto, string createdBy);
    }
}