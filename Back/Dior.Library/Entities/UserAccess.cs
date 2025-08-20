using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ACCESS")]
    public class UserAccess
    {
        [Key]
        public int Id { get; set; }

        public long UserId { get; set; }
        public int AccessId { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string? LastEditBy { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Access? Access { get; set; }
    }
}