using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.Access
{
    /// <summary>
    /// DTO pour les comp�tences d'acc�s
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
    /// DTO pour cr�er une comp�tence d'acc�s
    /// </summary>
    public class CreateAccessCompetencyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre � jour une comp�tence d'acc�s
    /// </summary>
    public class UpdateAccessCompetencyDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour lier un utilisateur � une comp�tence d'acc�s
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
    /// DTO pour cr�er une liaison utilisateur-comp�tence
    /// </summary>
    public class CreateUserAccessCompetencyDto
    {
        public int UserId { get; set; }
        public int AccessCompetencyId { get; set; }
    }

    /// <summary>
    /// DTO pour l'acc�s utilisateur
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
    /// DTO pour cr�er un acc�s utilisateur
    /// </summary>
    public class CreateUserAccessDto
    {
        public int UserId { get; set; }
        public int AccessId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre � jour un acc�s utilisateur
    /// </summary>
    public class UpdateUserAccessDto
    {
        public int? AccessId { get; set; }
    }
}