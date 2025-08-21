using System;

namespace Dior.Database.DTOs.AuditLog
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}