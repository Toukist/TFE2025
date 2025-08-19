namespace Dior.Service.Host.Models
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }  // Pour l'affichage
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
