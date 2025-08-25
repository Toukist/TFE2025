using System;
using System.Collections.Generic;

namespace Dior.Data.DTO.Team
{
    /// <summary>
    /// DTO pour les équipes
    /// </summary>
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// DTO pour créer une équipe
    /// </summary>
    public class CreateTeamDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre à jour une équipe
    /// </summary>
    public class UpdateTeamDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}