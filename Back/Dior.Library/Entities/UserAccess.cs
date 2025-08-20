using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    public partial class UserAccess
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long UserId { get; set; } 

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public int AccessId { get; set; }

        [ForeignKey(nameof(AccessId))]
        public virtual Access Access { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string LastEditBy { get; set; }
    }
}