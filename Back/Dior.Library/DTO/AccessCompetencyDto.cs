

namespace Dior.Library.DTO
{
    // DTO
    public class AccessCompetencyDto
    {
        public int Id { get; set; } // pour les GET
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } // utilisé en GET, pas en POST/PUT
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}
