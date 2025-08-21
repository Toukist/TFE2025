namespace Dior.Library.DTOs
{
    // FIX: DTO pour la s�lection de r�le
    public class SelectRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
    }
}