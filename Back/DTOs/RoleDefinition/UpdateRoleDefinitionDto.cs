namespace Dior.Database.DTOs.RoleDefinition
{
    public class UpdateRoleDefinitionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool IsActive { get; set; }
        public string LastEditBy { get; set; }
    }
}