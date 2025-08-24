using System.Security.Claims;
using Dior.Library.DTO.User;
using System.Collections.Generic;

namespace Dior.Service.Host.Services
{
    /// <summary>
    /// Interface pour le service de tokens JWT
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Génère un token JWT pour un utilisateur
        /// </summary>
        string GenerateToken(UserDto user);
        
        /// <summary>
        /// Génère un token JWT avec des paramètres spécifiques
        /// </summary>
        string GenerateToken(string userId, string username, List<string>? roles = null);
        
        /// <summary>
        /// Valide un token JWT et retourne les claims
        /// </summary>
        ClaimsPrincipal? ValidateToken(string token);
        
        /// <summary>
        /// Extrait l'ID utilisateur depuis un token
        /// </summary>
        long? GetUserIdFromToken(string token);
        
        /// <summary>
        /// Vérifie si un token est expiré
        /// </summary>
        bool IsTokenExpired(string token);
    }
}
