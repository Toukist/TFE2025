using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom d'utilisateur doit contenir entre 2 et 100 caract�res")]
        public string UserName { get; set; } = string.Empty;

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

        [StringLength(255, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 255 caract�res")]
        public string? Password { get; set; }

        /// <summary>
        /// Identifiant d��quipe (optionnel)
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// Num�ro de badge (optionnel)
        /// </summary>
        public string? BadgePhysicalNumber { get; set; }

        /// <summary>
        /// R�les associ�s (liste d'ID de r�les)
        /// </summary>
        public List<int> RoleIds { get; set; } = new();

        /// <summary>
        /// D�finit si l�utilisateur est actif
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
