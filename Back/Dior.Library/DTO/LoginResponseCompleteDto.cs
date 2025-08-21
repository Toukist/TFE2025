using Dior.Library.DTO.Users;
using Dior.Library.DTOs;
namespace Dior.Library.DTOs
{
    public class LoginResponseCompleteDto
    {
        public string Token { get; set; } = string.Empty;
        public UserFullDto User { get; set; } = null!;
        public List<RoleDefinitionDto> Roles { get; set; } = new();
        public List<PrivilegeDto> Privileges { get; set; } = new();
        public DateTime TokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
