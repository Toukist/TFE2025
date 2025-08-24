using System;
using System.ComponentModel.DataAnnotations;
using Dior.Library.DTO.User;

namespace Dior.Library.DTO.Auth
{
    /// <summary>
    /// DTO pour les requêtes de connexion - Support badge ET credentials
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Nom d'utilisateur (optionnel si badge fourni)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Mot de passe (optionnel si badge fourni)
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Numéro de badge physique (optionnel si username/password fournis)
        /// </summary>
        public string? BadgePhysicalNumber { get; set; }

        /// <summary>
        /// Validation : soit badge, soit username+password
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(BadgePhysicalNumber) ||
                   (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
        }
    }

    /// <summary>
    /// DTO pour les réponses de connexion simple
    /// </summary>
    public class LoginResponseDto
    {
        public UserDto User { get; set; } = new();
        public string Token { get; set; } = "";
        public List<RoleDefinitionDto> Roles { get; set; } = new();
        public List<string> AccessCompetencies { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO pour les réponses de connexion complètes avec tous les détails
    /// </summary>
    public class LoginResponseCompleteDto
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> AccessCompetencies { get; set; } = new();
        public bool IsActive { get; set; }
        public string? Phone { get; set; }
        public string? BadgePhysicalNumber { get; set; }
    }

    /// <summary>
    /// DTO pour changer le mot de passe
    /// </summary>
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Le mot de passe actuel est obligatoire")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire")]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; } = "";
    }

    /// <summary>
    /// DTO pour réinitialiser le mot de passe
    /// </summary>
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = "";
    }
}
