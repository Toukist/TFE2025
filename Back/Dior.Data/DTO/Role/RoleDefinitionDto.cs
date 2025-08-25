using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Role
{
    /// <summary>
    /// DTO pour les définitions de rôles
    /// </summary>
    public class RoleDefinitionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentRoleId { get; set; }
        public string? ParentRoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        public List<PrivilegeDto>? Privileges { get; set; }
    }

    /// <summary>
    /// DTO pour créer une définition de rôle
    /// </summary>
    public class CreateRoleDefinitionDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre à jour une définition de rôle
    /// </summary>
    public class UpdateRoleDefinitionDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour créer un rôle (alias pour compatibilité)
    /// </summary>
    public class CreateRoleDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre à jour un rôle (alias pour compatibilité)
    /// </summary>
    public class UpdateRoleDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}