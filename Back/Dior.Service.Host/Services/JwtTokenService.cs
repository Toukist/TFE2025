using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dior.Library.DTO.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dior.Service.Host.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string userName, List<string>? roles = null)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret manquant dans la configuration.");

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT secret trop court. Il faut au moins 32 caractères.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim("userId", userId)
            };

            // Ajouter les rôles
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (!string.IsNullOrWhiteSpace(role))
                        claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "480"); // 8 heures par défaut

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(UserDto user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return GenerateToken(user.Id.ToString(), user.UserName, user.Roles);
        }
    }
}