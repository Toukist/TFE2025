using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.Role
{
    /// <summary>
    /// DTO pour les privil�ges
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
    /// DTO pour cr�er un privil�ge
    /// </summary>
    public class CreatePrivilegeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre � jour un privil�ge
    /// </summary>
    public class UpdatePrivilegeDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour les privil�ges de r�le
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
    /// DTO pour cr�er un privil�ge de r�le
    /// </summary>
    public class CreateRoleDefinitionPrivilegeDto
    {
        public int RoleDefinitionId { get; set; }
        public int PrivilegeId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre � jour un privil�ge de r�le
    /// </summary>
    public class UpdateRoleDefinitionPrivilegeDto
    {
        public int? PrivilegeId { get; set; }
    }
}