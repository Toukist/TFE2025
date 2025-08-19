namespace Dior.Library.DTO
{
    public class LoginResponseCompleteDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public List<RoleDefinitionDto> Roles { get; set; }
        public List<PrivilegeDto> Privileges { get; set; }
    }
}
