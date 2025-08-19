using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Users
{
    // Create : sans IsActive
    public class CreateUserDtoBase
    {
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public int? TeamId { get; set; }
        public string? Password { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    // Update : avec IsActive pour pouvoir (dé)activer
    public class UpdateUserDto : CreateUserDtoBase
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }  // <- ici oui
    }
}
