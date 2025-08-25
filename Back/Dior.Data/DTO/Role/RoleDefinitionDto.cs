using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Role
{
    /// <summary>
    /// DTO pour les d�finitions de r�les
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
    /// DTO pour cr�er une d�finition de r�le
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
    /// DTO pour mettre � jour une d�finition de r�le
    /// </summary>
    public class UpdateRoleDefinitionDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour cr�er un r�le (alias pour compatibilit�)
    /// </summary>
    public class CreateRoleDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre � jour un r�le (alias pour compatibilit�)
    /// </summary>
    public class UpdateRoleDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}