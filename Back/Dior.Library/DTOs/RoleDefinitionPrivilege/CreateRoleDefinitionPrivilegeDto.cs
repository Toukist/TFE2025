namespace Dior.Database.DTOs.RoleDefinitionPrivilege
{
    public class CreateRoleDefinitionPrivilegeDto
    {
        public int RoleDefinitionId { get; set; }
        public int PrivilegeId { get; set; }
        public string CreatedBy { get; set; }
    }
}