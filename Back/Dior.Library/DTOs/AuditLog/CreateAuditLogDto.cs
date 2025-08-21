namespace Dior.Database.DTOs.AuditLog
{
    public class CreateAuditLogDto
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string Details { get; set; }
    }
}