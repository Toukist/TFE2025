using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ROLE")]
    public class UserRole
    {
        [Key]
        public int Id { get; set; } // Chang� de long � int pour coh�rence

        public int RoleDefinitionId { get; set; } // Chang� de long � int
        public int UserId { get; set; } // Chang� de long � int

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ajout�
        
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty; // Ajout�

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