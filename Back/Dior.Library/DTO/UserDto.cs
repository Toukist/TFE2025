using System;
using System.Collections.Generic;

namespace Dior.Library.DTO
{
    public class UserDto
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TeamName { get; set; }
        public int? BadgePhysicalNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        public List<RoleDefinitionDto> Roles { get; set; } = new();
    }
}
