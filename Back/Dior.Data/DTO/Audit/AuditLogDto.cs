namespace Dior.Library.DTO.Audit
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
    }

    public class CreateAuditLogDto
    {
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string? Details { get; set; }
        public long UserId { get; set; }
    }
}
