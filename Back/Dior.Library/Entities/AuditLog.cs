using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("AUDIT_LOG")]
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } = string.Empty; // Chang� de TableName � EntityType

        public long EntityId { get; set; } // Chang� de RecordId � EntityId

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public long UserId { get; set; } // Chang� de long? � int

        public DateTime Timestamp { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; } // Ajout�

        [MaxLength(500)]
        public string? UserAgent { get; set; } // Ajout�

        // Navigation
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}