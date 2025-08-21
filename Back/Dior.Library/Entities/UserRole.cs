using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ROLE")]
    public class UserRole
    {
        [Key]
        public int Id { get; set; } // Changé de long à int pour cohérence

        public int RoleDefinitionId { get; set; } // Changé de long à int
        public int UserId { get; set; } // Changé de long à int

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