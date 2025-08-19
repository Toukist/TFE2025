using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Database.Entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public string Action { get; set; }
        
        [Required]
        public string TableName { get; set; }
        
        [Required]
        public int RecordId { get; set; }
        
        public string Details { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}