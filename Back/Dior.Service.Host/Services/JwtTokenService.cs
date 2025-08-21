using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dior.Library.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dior.Service.Host.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IConfiguration _configuration;

        public JwtTokenService(JwtOptions jwtOptions, IConfiguration configuration)
        {
            _jwtOptions = jwtOptions;
            _configuration = configuration;
        }

        public string GenerateToken(UserDto user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var secret = _jwtOptions.Secret;
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret manquant dans JwtOptions.");

            byte[] keyBytes;
            try { keyBytes = Convert.FromBase64String(secret); }
            catch { keyBytes = Encoding.UTF8.GetBytes(secret); }

            // Exige de nouveau ≥ 32 octets (256 bits)
            if (keyBytes.Length < 32)
                throw new InvalidOperationException(
                    $"JWT secret trop court: {keyBytes.Length * 8} bits. Il faut ≥ 256 bits (32 octets). Génère une clé base64 de 32 octets.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty)
            };

            // FIX: user.Roles est maintenant List<RoleDefinitionDto>
            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    if (role != null && !string.IsNullOrWhiteSpace(role.Name))
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var issuer = _jwtOptions.Issuer ?? _configuration["Jwt:Issuer"];
            var audience = _jwtOptions.Audience ?? _configuration["Jwt:Audience"];
            var expirationMinutes = _jwtOptions.ExpirationMinutes > 0
                ? _jwtOptions.ExpirationMinutes
                : int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}