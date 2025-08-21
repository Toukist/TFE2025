namespace Dior.Library.DTOs
{
    public class TaskDto
    {
        public long Id { get; set; } // Changé en long pour cohérence
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public long AssignedToUserId { get; set; } // Changé en long
        public string? AssignedToUserName { get; set; } // Propriété calculée
        public long CreatedByUserId { get; set; } // Changé en long
        public string? CreatedByUserName { get; set; } // Propriété calculée
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; } // Ajout de DueDate
        public string? CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }
}
