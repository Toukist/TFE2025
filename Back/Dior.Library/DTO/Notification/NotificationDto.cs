using System;

namespace Dior.Library.DTO.Notification
{
    /// <summary>
    /// DTO pour les notifications
    /// </summary>
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Info, Warning, Error, Success
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }
    }

    /// <summary>
    /// DTO pour créer une notification
    /// </summary>
    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public int? UserId { get; set; }
        public string? ActionUrl { get; set; }
    }

    /// <summary>
    /// Classe pour les requêtes de notification (compatibility)
    /// </summary>
    public class CreateNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public int? UserId { get; set; }
        public string? ActionUrl { get; set; }
    }
}