using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class CreateNotificationRequest
    {
        [Required(ErrorMessage = "L'ID utilisateur est obligatoire")]
        public long UserId { get; set; }
        
        [Required(ErrorMessage = "Le type de notification est obligatoire")]
        [RegularExpression("^(ALERT|INFO|WARNING)$", ErrorMessage = "Type invalide. Valeurs autoris�es: ALERT, INFO, WARNING")]
        public string Type { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le message est obligatoire")]
        [StringLength(1000, ErrorMessage = "Le message ne peut pas d�passer 1000 caract�res")]
        public string Message { get; set; } = string.Empty;
    }

    public class BulkNotificationRequest
    {
        [Required(ErrorMessage = "La liste des utilisateurs est obligatoire")]
        [MinLength(1, ErrorMessage = "Au moins un utilisateur doit �tre sp�cifi�")]
        public List<long> UserIds { get; set; } = new();
        
        [Required(ErrorMessage = "Le type de notification est obligatoire")]
        [RegularExpression("^(ALERT|INFO|WARNING)$", ErrorMessage = "Type invalide. Valeurs autoris�es: ALERT, INFO, WARNING")]
        public string Type { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le message est obligatoire")]
        [StringLength(1000, ErrorMessage = "Le message ne peut pas d�passer 1000 caract�res")]
        public string Message { get; set; } = string.Empty;
    }
}