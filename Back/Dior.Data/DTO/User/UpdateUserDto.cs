using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Dior.Library.DTO.User
{
    /// <summary>
    /// DTO pour mettre à jour un utilisateur
    /// </summary>
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
        public int? TeamId { get; set; }
        public List<int>? RoleIds { get; set; }
        public string? BadgePhysicalNumber { get; set; }
    }
}