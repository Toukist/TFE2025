using System;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Role
{
    /// <summary>
    /// DTO pour les rôles utilisateur
    /// </summary>
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int RoleDefinitionId { get; set; }
        public string? RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer un rôle utilisateur
    /// </summary>
    public class CreateUserRoleDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleDefinitionId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour un rôle utilisateur
    /// </summary>
    public class UpdateUserRoleDto
    {
        public int? RoleDefinitionId { get; set; }
    }

    /// <summary>
    /// DTO pour assigner un rôle
    /// </summary>
    public class AssignRoleDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleDefinitionId { get; set; }
    }
}