using Dior.Library.DTO.Notification;

namespace Dior.Service.Host.Services
{
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task<List<NotificationDto>> GetByUserIdAsync(int userId)
        {
            var notifications = new List<NotificationDto>
            {
                new NotificationDto
                {
                    Id = 1,
                    Title = "Bienvenue",
                    Message = "Bienvenue dans l'application Dior !",
                    Type = "Info",
                    UserId = userId,
                    UserName = "System",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                }
            };
            return Task.FromResult(notifications);
        }

        public Task<List<NotificationDto>> GetUnreadByUserIdAsync(int userId)
        {
            var notifications = new List<NotificationDto>
            {
                new NotificationDto
                {
                    Id = 1,
                    Title = "Notification non lue",
                    Message = "Vous avez une nouvelle notification",
                    Type = "Warning",
                    UserId = userId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                }
            };
            return Task.FromResult(notifications);
        }

        public Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
        {
            var notification = new NotificationDto
            {
                Id = new Random().Next(1000, 9999),
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                UserId = dto.UserId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            return Task.FromResult(notification);
        }

        public Task<bool> MarkAsReadAsync(int id)
        {
            // Version mockée
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int id)
        {
            // Version mockée
            return Task.FromResult(true);
        }
    }
}