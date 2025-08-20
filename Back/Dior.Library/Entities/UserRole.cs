using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ROLE")]
    public class UserRole
    {
        [Key]
        public long Id { get; set; } // BIGINT dans la base

        public long RoleDefinitionId { get; set; } // BIGINT
        public long UserId { get; set; } // BIGINT

        [Required]
        [MaxLength(50)]
        public string LastEditBy { get; set; } = string.Empty;

        public DateTime LastEditAt { get; set; }

        // Navigation properties (utiliser int pour les FK car User.Id est int)
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("RoleDefinitionId")]
        public virtual RoleDefinition? RoleDefinition { get; set; }
    }
}