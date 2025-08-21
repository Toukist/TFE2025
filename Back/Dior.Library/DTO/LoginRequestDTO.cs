using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    /// <summary>
    /// Objet de transfert pour une tentative de connexion.
    /// Peut contenir un couple identifiant/mot de passe ou un numéro de badge physique.
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// Nom d'utilisateur (requis si badgePhysicalNumber n'est pas fourni)
        /// </summary>
        /// <example>admin</example>
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe (requis avec username)
        /// </summary>
        /// <example>admin123</example>
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Numéro physique du badge (alternatif à username/password)
        /// </summary>
        /// <example>12345</example>
        public int? BadgePhysicalNumber { get; set; }
    }
}
