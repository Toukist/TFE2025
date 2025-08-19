using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas d�passer 200 caract�res")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "La description ne peut pas d�passer 1000 caract�res")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Termin�e|Annul�e)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = "En attente";
        
        [Required(ErrorMessage = "L'utilisateur assign� est obligatoire")]
        public long AssignedToUserId { get; set; }
        
        [Required(ErrorMessage = "L'utilisateur cr�ateur est obligatoire")]
        public long CreatedByUserId { get; set; }
        
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskRequest
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas d�passer 200 caract�res")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "La description ne peut pas d�passer 1000 caract�res")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Termin�e|Annul�e)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "L'utilisateur assign� est obligatoire")]
        public long AssignedToUserId { get; set; }
        
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Termin�e|Annul�e)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = string.Empty;
    }

    public class AssignTaskRequest
    {
        [Required(ErrorMessage = "L'utilisateur assign� est obligatoire")]
        public long AssignedToUserId { get; set; }
    }
}