namespace Dior.Database.DTOs.RoleDefinition
{
    public class CreateRoleDefinitionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}