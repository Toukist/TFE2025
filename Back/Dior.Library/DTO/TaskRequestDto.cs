using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Terminée|Annulée)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = "En attente";
        
        [Required(ErrorMessage = "L'utilisateur assigné est obligatoire")]
        public long AssignedToUserId { get; set; }
        
        [Required(ErrorMessage = "L'utilisateur créateur est obligatoire")]
        public long CreatedByUserId { get; set; }
        
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskRequest
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Terminée|Annulée)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "L'utilisateur assigné est obligatoire")]
        public long AssignedToUserId { get; set; }
        
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Terminée|Annulée)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = string.Empty;
    }

    public class AssignTaskRequest
    {
        [Required(ErrorMessage = "L'utilisateur assigné est obligatoire")]
        public long AssignedToUserId { get; set; }
    }
}