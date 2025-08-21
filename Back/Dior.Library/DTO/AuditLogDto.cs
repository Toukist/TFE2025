namespace Dior.Library.DTOs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public long? UserId { get; set; } // Correspond à l'entité
        public string? Username { get; set; }  // Pour l'affichage
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int? RecordId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
