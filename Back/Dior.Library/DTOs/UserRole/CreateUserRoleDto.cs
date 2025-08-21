using System;

namespace Dior.Database.DTOs.UserRole
{
    public class CreateUserRoleDto
    {
        public int UserId { get; set; }
        public int RoleDefinitionId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string CreatedBy { get; set; }
    }
}