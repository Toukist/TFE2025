using System;

namespace Dior.Library.DTO.Audit
{
    /// <summary>
    /// DTO pour les logs d'audit
    /// </summary>
    public class AuditLogDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// DTO pour créer un log d'audit
    /// </summary>
    public class CreateAuditLogDto
    {
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public int UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}