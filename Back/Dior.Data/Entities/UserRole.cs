using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ROLE")]
    public class UserRole
    {
        [Key]
        public long Id { get; set; } // Converti en long

        public long RoleDefinitionId { get; set; } // Converti en long
        public long UserId { get; set; } // Converti en long

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ajouté
        
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty; // Ajouté

        [Required]
        [MaxLength(50)]
        public string LastEditBy { get; set; } = string.Empty;

        public DateTime LastEditAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("RoleDefinitionId")]
        public virtual RoleDefinition? RoleDefinition { get; set; }
    }
}