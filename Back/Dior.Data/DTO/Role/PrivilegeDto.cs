using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.Role
{
    /// <summary>
    /// DTO pour les privilèges
    /// </summary>
    public class PrivilegeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }

    /// <summary>
    /// DTO pour créer un privilège
    /// </summary>
    public class CreatePrivilegeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre à jour un privilège
    /// </summary>
    public class UpdatePrivilegeDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour les privilèges de rôle
    /// </summary>
    public class RoleDefinitionPrivilegeDto
    {
        public int Id { get; set; }
        public int RoleDefinitionId { get; set; }
        public int PrivilegeId { get; set; }
        public string? RoleName { get; set; }
        public string? PrivilegeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer un privilège de rôle
    /// </summary>
    public class CreateRoleDefinitionPrivilegeDto
    {
        public int RoleDefinitionId { get; set; }
        public int PrivilegeId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour un privilège de rôle
    /// </summary>
    public class UpdateRoleDefinitionPrivilegeDto
    {
        public int? PrivilegeId { get; set; }
    }
}