namespace Dior.Library.DTO
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int RoleDefinitionID { get; set; }
        public int UserID { get; set; }
        public string? LastEditBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
