using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("AUDITLOGS")] // cohérent avec ta table SQL
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long UserId { get; set; } // BIGINT

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        public int RecordId { get; set; }

        public string? Details { get; set; }

        public DateTime Timestamp { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
