using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "L'ID est obligatoire")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom d'utilisateur doit contenir entre 2 et 100 caract�res")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le pr�nom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le pr�nom ne peut pas d�passer 100 caract�res")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom de famille est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom de famille ne peut pas d�passer 100 caract�res")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [StringLength(255, ErrorMessage = "L'email ne peut pas d�passer 255 caract�res")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Le num�ro de t�l�phone n'est pas valide")]
        [StringLength(50, ErrorMessage = "Le t�l�phone ne peut pas d�passer 50 caract�res")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
        
        public int? TeamId { get; set; }
        
        public List<int> RoleIds { get; set; } = new();
    }
}