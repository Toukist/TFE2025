namespace Dior.Library.DTOs
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int RoleDefinitionId { get; set; }
        public long UserId { get; set; }
        public string? LastEditBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        
        // Propriétés additionnelles pour l'affichage
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
