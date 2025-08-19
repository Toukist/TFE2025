using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dior.Library.Entities
{
    [Table("AUDIT_LOGS")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Action { get; set; }

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; }

        [Required]
        public int RecordId { get; set; }

        public string Details { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}