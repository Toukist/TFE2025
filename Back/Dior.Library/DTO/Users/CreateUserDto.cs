using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Users
{
    public class CreateUserDto
    {
        public bool IsActive { get; set; } = true;
        [Required, MaxLength(100)] public string UserName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(255)] public string Email { get; set; } = string.Empty;
        [MaxLength(50)] public string? Phone { get; set; }
        public int? TeamId { get; set; }
        [MaxLength(200)] public string? Password { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}
