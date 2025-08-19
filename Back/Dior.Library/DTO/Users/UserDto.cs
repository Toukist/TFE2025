using System;
using System.Collections.Generic;

namespace Dior.Library.DTO.Users
{
    // DTO dédié aux endpoints /api/users (ADO.NET SP based)
    public class UserDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }
}
