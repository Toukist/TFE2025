using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom d'utilisateur doit contenir entre 2 et 100 caractères")]
        public string UserName { get; set; } = string.Empty;

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

        [StringLength(255, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 255 caractères")]
        public string? Password { get; set; }

        /// <summary>
        /// Identifiant d’équipe (optionnel)
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// Numéro de badge (optionnel)
        /// </summary>
        public string? BadgePhysicalNumber { get; set; }

        /// <summary>
        /// Rôles associés (liste d'ID de rôles)
        /// </summary>
        public List<int> RoleIds { get; set; } = new();

        /// <summary>
        /// Définit si l’utilisateur est actif
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
