namespace Dior.Library.DTO
{
    public class PrivilegeDto
    {
        public int Id { get; set; } // Corrigé de long à int pour correspondre à l'entité
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } // Simplifié pour correspondre à l'entité
        public string LastEditBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
