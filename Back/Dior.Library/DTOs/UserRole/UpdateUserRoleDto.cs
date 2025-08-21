using System;

namespace Dior.Database.DTOs.UserRole
{
    public class UpdateUserRoleDto
    {
        public DateTime? ExpiresAt { get; set; }
        public string LastEditBy { get; set; }
    }
}