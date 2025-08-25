using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.User
{
    /// <summary>
    /// DTO principal pour l'utilisateur
    /// </summary>
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public long? TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? BadgePhysicalNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}