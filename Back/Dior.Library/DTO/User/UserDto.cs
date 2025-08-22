using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.User
{
    /// <summary>
    /// DTO principal pour les utilisateurs - correspond exactement à la structure DB
    /// </summary>
    public class UserDto
    {
        public long Id { get; set; }                    // BIGINT de la DB
        public string Username { get; set; } = "";      // Correspond à DB.Username
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? BadgePhysicalNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        public List<RoleDefinitionDto>? Roles { get; set; }      // Rôles complets
        public List<string>? AccessCompetencies { get; set; }    // Noms des accès
        // Pas de mot de passe dans les réponses
    }
}