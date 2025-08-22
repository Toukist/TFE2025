using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dior.Library.DTO.User;
using Dior.Library.DTO.Role;

namespace Dior.Service.Host.Services
{
    /// <summary>
    /// Options de configuration JWT
    /// </summary>
    public class JwtOptions
    {
        public string Secret { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int ExpirationMinutes { get; set; } = 60;
    }

    /// <summary>
    /// Service de gestion des tokens JWT pour l'authentification Dior
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly SymmetricSecurityKey _key;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration config, ILogger<JwtTokenService> logger)
        {
            _logger = logger;
            _jwtOptions = new JwtOptions
            {
                Secret = config["Jwt:Secret"] ?? "DiorSuperSecretKeyForJWTTokenGeneration2024!",
                Issuer = config["Jwt:Issuer"] ?? "DiorAPI",
                Audience = config["Jwt:Audience"] ?? "DiorFrontend",
                ExpirationMinutes = int.Parse(config["Jwt:ExpirationMinutes"] ?? "60")
            };
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        }

        /// <summary>
        /// Génère un token JWT complet pour un utilisateur avec tous ses détails
        /// </summary>
        public string GenerateToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new("firstName", user.FirstName),
                new("lastName", user.LastName),
                new("email", user.Email),
                new("isActive", user.IsActive.ToString().ToLower()),
                new("userId", user.Id.ToString()),
                new("username", user.Username)
            };

            // Ajouter les informations optionnelles
            if (!string.IsNullOrEmpty(user.Phone))
                claims.Add(new Claim("phone", user.Phone));

            if (user.TeamId.HasValue)
                claims.Add(new Claim("teamId", user.TeamId.Value.ToString()));

            if (!string.IsNullOrEmpty(user.TeamName))
                claims.Add(new Claim("teamName", user.TeamName));

            if (!string.IsNullOrEmpty(user.BadgePhysicalNumber))
                claims.Add(new Claim("badgeNumber", user.BadgePhysicalNumber));

            // Ajouter les rôles comme claims
            if (user.Roles != null && user.Roles.Count > 0)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    claims.Add(new Claim("role", role.Name)); // Claim alternatif
                }
            }

            // Ajouter les compétences d'accès
            if (user.AccessCompetencies != null && user.AccessCompetencies.Count > 0)
            {
                foreach (var competency in user.AccessCompetencies)
                {
                    claims.Add(new Claim("access_competency", competency));
                }
            }

            return GenerateJwtToken(claims);
        }

        /// <summary>
        /// Génère un token JWT avec des paramètres basiques
        /// </summary>
        public string GenerateToken(string userId, string username, List<string>? roles = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Name, username),
                new("userId", userId),
                new("username", username)
            };

            // Ajouter les rôles si fournis
            if (roles != null && roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return GenerateJwtToken(claims);
        }

        /// <summary>
        /// Valide un token JWT et retourne les claims
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Erreur lors de la validation du token: {Error}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Extrait l'ID utilisateur depuis un token
        /// </summary>
        public long? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             principal.FindFirst("userId")?.Value;

            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Vérifie si un token est expiré
        /// </summary>
        public bool IsTokenExpired(string token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Génère le token JWT final avec les claims fournis
        /// </summary>
        private string GenerateJwtToken(List<Claim> claims)
        {
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}