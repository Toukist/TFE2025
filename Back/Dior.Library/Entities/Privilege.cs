using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("Privileges")]
    public class Privilege
    {
        public Privilege()
        {
            RoleDefinitionPrivileges = new List<RoleDefinitionPrivilege>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Navigation
        public ICollection<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; } = null!;
    }
}