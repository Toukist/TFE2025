namespace Dior.Library.DTO
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public long UserId { get; set; } // Chang� en long pour coh�rence
        public string Type { get; set; } = string.Empty; // ALERT, INFO, WARNING
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; } // Ajout de CreatedBy
    }
}