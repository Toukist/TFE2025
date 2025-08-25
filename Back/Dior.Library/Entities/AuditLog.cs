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
        public string EntityType { get; set; } = string.Empty; // Changé de TableName à EntityType

        public long EntityId { get; set; } // Changé de RecordId à EntityId

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public long UserId { get; set; } // Changé de long? à int

        public DateTime Timestamp { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; } // Ajouté

        [MaxLength(500)]
        public string? UserAgent { get; set; } // Ajouté

        // Navigation
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}