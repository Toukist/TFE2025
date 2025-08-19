#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO complet pour l'utilisateur, utilisé pour l'édition et la consultation détaillée.
    /// Hérite de UserDto et ajoute des informations étendues.
    /// </summary>
    public class UserFullDto : UserDto
    {
        /// <summary>
        /// Informations de l'équipe complète
        /// </summary>
        public TeamDto? Team { get; set; }

        /// <summary>
        /// Liste des définitions de rôles complètes (en plus des noms dans Roles)
        /// </summary>
        public List<RoleDefinitionDto>? RoleDefinitions { get; set; }

        /// <summary>
        /// Liste des compétences d'accès
        /// </summary>
        public List<AccessCompetencyDto>? AccessCompetencies { get; set; }

        /// <summary>
        /// Notifications récentes
        /// </summary>
        public List<NotificationDto>? RecentNotifications { get; set; }
    }
}
