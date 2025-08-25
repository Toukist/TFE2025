using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.User
{
    /// <summary>
    /// DTO complet pour l'utilisateur avec toutes les informations détaillées
    /// </summary>
    public class UserFullDto : UserDto
    {
        /// <summary>
        /// Informations complètes de l'équipe
        /// </summary>
        public TeamDto? Team { get; set; }

        /// <summary>
        /// Définitions de rôles complètes
        /// </summary>
        public List<RoleDefinitionDto>? RoleDefinitions { get; set; }

        /// <summary>
        /// Compétences d'accès
        /// </summary>
        public List<AccessCompetencyDto>? AccessCompetencies { get; set; }

        /// <summary>
        /// Accès utilisateur
        /// </summary>
        public List<UserAccessDto>? UserAccesses { get; set; }
    }

    /// <summary>
    /// DTO simplifié pour affichage en liste
    /// </summary>
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? TeamName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}