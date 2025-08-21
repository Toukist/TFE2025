using Dior.Library.DTO.User;
using System.Collections.Generic;

namespace Dior.Service.Host.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, string userName, List<string>? roles = null);
        string GenerateToken(UserDto user);
    }
}
