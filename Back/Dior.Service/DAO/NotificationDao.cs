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

        public List<NotificationDto> GetByUserId(long userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = (int)n.Id, // Correction : cast explicite de long vers int
                    UserId = (int?)n.UserId, // Correction : cast explicite de long vers int?
                    Type = n.Type,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public void MarkAsRead(long id)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == id);
            if (notif != null)
            {
                notif.IsRead = true;
                _context.SaveChanges();
            }
        }

        public void Delete(long id)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == id);
            if (notif != null)
            {
                _context.Notifications.Remove(notif);
                _context.SaveChanges();
            }
        }

        public long Add(NotificationDto dto, string createdBy)
        {
            var notif = new Notification
            {
                UserId = dto.UserId.HasValue ? dto.UserId.Value : 0, // Correction : conversion int? vers long
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