namespace Dior.Library.DTO
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int RoleDefinition { get; set; }  // Corrigé: pas de suffixe ID
        public int User { get; set; }  // Corrigé: pas de suffixe ID
        public string LastEditBy { get; set; } = string.Empty;
        public DateTime LastEditAt { get; set; }
        
        // Propriétés additionnelles pour l'affichage
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
