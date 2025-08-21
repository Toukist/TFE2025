using System.Security.Claims;
using System.Text.Json;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    /// <summary>
    /// Service d'audit minimal pour tracer les mutations cl�s
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Enregistre une action d'audit
        /// </summary>
        Task LogAsync(int userId, string action, string entity, int entityId, object? before = null, object? after = null);

        /// <summary>
        /// Enregistre une action d'audit avec d�tection automatique de l'utilisateur depuis le contexte HTTP
        /// </summary>
        Task LogAsync(ClaimsPrincipal user, string action, string entity, int entityId, object? before = null, object? after = null);

        /// <summary>
        /// Enregistre une cr�ation
        /// </summary>
        Task LogCreateAsync(int userId, string entity, int entityId, object after);

        /// <summary>
        /// Enregistre une mise � jour
        /// </summary>
        Task LogUpdateAsync(int userId, string entity, int entityId, object before, object after);

        /// <summary>
        /// Enregistre une suppression
        /// </summary>
        Task LogDeleteAsync(int userId, string entity, int entityId, object before);
    }
}