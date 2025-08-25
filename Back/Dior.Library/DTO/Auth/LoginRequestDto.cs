using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Auth
{
    /// <summary>
    /// DTO pour les demandes de connexion
    /// </summary>
    public class LoginRequestDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? BadgePhysicalNumber { get; set; }
    }

    /// <summary>
    /// DTO pour les réponses de connexion
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// DTO complet pour les réponses de connexion avec informations utilisateur
    /// </summary>
    public class LoginResponseCompleteDto : LoginResponseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long? TeamId { get; set; }
        public string? TeamName { get; set; }
    }
}