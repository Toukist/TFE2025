using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("AUDIT_LOG")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        public int? RecordId { get; set; }

        public string? Details { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public long? UserId { get; set; }

        public DateTime Timestamp { get; set; }
        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation
        public virtual User? User { get; set; }
    }
}