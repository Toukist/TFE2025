using System;

namespace Dior.Database.DTOs.UserRole
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleDefinitionId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}