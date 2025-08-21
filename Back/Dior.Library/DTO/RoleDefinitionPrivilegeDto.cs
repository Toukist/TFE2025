namespace Dior.Library.DTOs
{
    public class RoleDefinitionPrivilegeDto
    {
        public int Id { get; set; }
        public int RoleDefinitionId { get; set; }
        public int PrivilegeId { get; set; }
        public string? RoleDefinitionName { get; set; }
        public string? PrivilegeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }
}