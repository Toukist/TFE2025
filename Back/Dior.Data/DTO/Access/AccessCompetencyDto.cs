using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.Access
{
    /// <summary>
    /// DTO pour les compétences d'accès
    /// </summary>
    public class AccessCompetencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }

    /// <summary>
    /// DTO pour créer une compétence d'accès
    /// </summary>
    public class CreateAccessCompetencyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre à jour une compétence d'accès
    /// </summary>
    public class UpdateAccessCompetencyDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour lier un utilisateur à une compétence d'accès
    /// </summary>
    public class UserAccessCompetencyDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AccessCompetencyId { get; set; }
        public string? UserName { get; set; }
        public string? AccessCompetencyName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer une liaison utilisateur-compétence
    /// </summary>
    public class CreateUserAccessCompetencyDto
    {
        public int UserId { get; set; }
        public int AccessCompetencyId { get; set; }
    }

    /// <summary>
    /// DTO pour l'accès utilisateur
    /// </summary>
    public class UserAccessDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AccessId { get; set; }
        public string? UserName { get; set; }
        public string? BadgePhysicalNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer un accès utilisateur
    /// </summary>
    public class CreateUserAccessDto
    {
        public int UserId { get; set; }
        public int AccessId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour un accès utilisateur
    /// </summary>
    public class UpdateUserAccessDto
    {
        public int? AccessId { get; set; }
    }
}