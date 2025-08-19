using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    /// <summary>
    /// Objet de transfert pour une tentative de connexion.
    /// Peut contenir un couple identifiant/mot de passe ou un numéro de badge physique.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Nom d'utilisateur (requis si badgePhysicalNumber n'est pas fourni)
        /// </summary>
        /// <example>admin</example>
        public string? Username { get; set; }

        /// <summary>
        /// Mot de passe (requis avec username)
        /// </summary>
        /// <example>admin123</example>
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        /// <summary>
        /// Numéro physique du badge (alternatif à username/password)
        /// </summary>
        /// <example>12345</example>
        public string? BadgePhysicalNumber { get; set; }
    }
}
