namespace Dior.Library.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
        public List<string> Privileges { get; set; } = new();
    }
}
