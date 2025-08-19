using System;

namespace Dior.Database.DTOs.RoleDefinitionPrivilege
{
    public class RoleDefinitionPrivilegeDto
    {
        public int Id { get; set; }
        public int RoleDefinitionId { get; set; }
        public int PrivilegeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}