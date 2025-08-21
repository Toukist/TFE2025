using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "L'ID est obligatoire")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom d'utilisateur doit contenir entre 2 et 100 caractères")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom de famille est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom de famille ne peut pas dépasser 100 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [StringLength(255, ErrorMessage = "L'email ne peut pas dépasser 255 caractères")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide")]
        [StringLength(50, ErrorMessage = "Le téléphone ne peut pas dépasser 50 caractères")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
        
        public int? TeamId { get; set; }
        
        public List<int> RoleIds { get; set; } = new();
    }
}