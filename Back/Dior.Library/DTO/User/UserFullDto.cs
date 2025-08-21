using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.User
{
    /// <summary>
    /// DTO complet pour l'utilisateur avec toutes les informations d�taill�es
    /// </summary>
    public class UserFullDto : UserDto
    {
        /// <summary>
        /// Informations compl�tes de l'�quipe
        /// </summary>
        public TeamDto? Team { get; set; }

        /// <summary>
        /// D�finitions de r�les compl�tes
        /// </summary>
        public List<RoleDefinitionDto>? RoleDefinitions { get; set; }

        /// <summary>
        /// Comp�tences d'acc�s
        /// </summary>
        public List<AccessCompetencyDto>? AccessCompetencies { get; set; }

        /// <summary>
        /// Acc�s utilisateur
        /// </summary>
        public List<UserAccessDto>? UserAccesses { get; set; }
    }

    /// <summary>
    /// DTO simplifi� pour affichage en liste
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