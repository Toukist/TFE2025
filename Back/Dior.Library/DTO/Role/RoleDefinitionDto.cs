using System;
using System.Collections.Generic;

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
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int>? PrivilegeIds { get; set; }
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
        public List<int>? PrivilegeIds { get; set; }
    }

    /// <summary>
    /// DTO pour les r�les utilisateur
    /// </summary>
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleDefinitionId { get; set; }
        public string? UserName { get; set; }
        public string? RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour cr�er un r�le utilisateur
    /// </summary>
    public class CreateUserRoleDto
    {
        public int UserId { get; set; }
        public int RoleDefinitionId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre � jour un r�le utilisateur
    /// </summary>
    public class UpdateUserRoleDto
    {
        public int? RoleDefinitionId { get; set; }
    }
}