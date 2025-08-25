using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Dior.Library.DTO.User
{
    /// <summary>
    /// DTO pour créer un nouvel utilisateur
    /// </summary>
    public class CreateUserDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
        
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        public int? TeamId { get; set; }
        public List<int> RoleIds { get; set; } = new();
        public string? BadgePhysicalNumber { get; set; }
    }
}