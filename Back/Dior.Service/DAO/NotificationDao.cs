using Dior.Library.DTO;
using Dior.Library.Entities;
using Dior.Library.Service.DAO;
using Dior.Service.Services;

namespace Dior.Service.DAO
{
    public class NotificationDao : INotificationDao
    {
        private readonly DiorDbContext _context;
        public NotificationDao(DiorDbContext context)
        {
            _context = context;
        }

        public List<NotificationDto> GetByUserId(int userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId, // long <- long maintenant
                    Type = n.Type,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public void MarkAsRead(int id)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == id);
            if (notif != null)
            {
                notif.IsRead = true;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == id);
            if (notif != null)
            {
                _context.Notifications.Remove(notif);
                _context.SaveChanges();
            }
        }

        public int Add(NotificationDto dto, string createdBy)
        {
            var notif = new Notification
            {
                UserId = (int)dto.UserId, // Conversion explicite long -> int pour la DB
                Type = dto.Type,
                Message = dto.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notif);
            _context.SaveChanges();
            return notif.Id;
        }
    }
}