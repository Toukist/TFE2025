using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("ROLE_DEFINITION_PRIVILEGE")]
    public class RoleDefinitionPrivilege
    {
        [Key]
        public int Id { get; set; }

        public int RoleDefinitionId { get; set; }
        [ForeignKey(nameof(RoleDefinitionId))]
        public RoleDefinition RoleDefinition { get; set; }

        public int PrivilegeId { get; set; }
        [ForeignKey(nameof(PrivilegeId))]
        public Privilege Privilege { get; set; }

        public DateTime CreatedAt { get; set; }
        [MaxLength(100)]
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        [MaxLength(100)]
        public string LastEditBy { get; set; }
    }
}