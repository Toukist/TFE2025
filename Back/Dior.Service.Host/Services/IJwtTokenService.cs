using Dior.Library.DTO;

public interface IJwtTokenService
{
    string GenerateToken(UserDto user);
}
