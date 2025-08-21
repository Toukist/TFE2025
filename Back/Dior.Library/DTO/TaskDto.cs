namespace Dior.Library.DTOs
{
    public class TaskDto
    {
        public long Id { get; set; } // Chang� en long pour coh�rence
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public long AssignedToUserId { get; set; } // Chang� en long
        public string? AssignedToUserName { get; set; } // Propri�t� calcul�e
        public long CreatedByUserId { get; set; } // Chang� en long
        public string? CreatedByUserName { get; set; } // Propri�t� calcul�e
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; } // Ajout de DueDate
        public string? CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }
}
