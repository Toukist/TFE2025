using Dior.Library.DTO;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using Dior.Service; // pour IRoleService
using Swashbuckle.AspNetCore.Annotations;

namespace Dior.Service.Host.Controllers
{
    /// <summary>
    /// Contrôleur d'authentification pour le système Dior Enterprise
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    [SwaggerTag("🔐 Authentification - Obtenir un token JWT pour accéder à l'API")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRoleService _roleService;

        public AuthController(IConfiguration config, IJwtTokenService jwtTokenService, IRoleService roleService)
        {
            _connectionString = config.GetConnectionString("DIOR_DB")
                ?? throw new InvalidOperationException("ConnectionStrings:DIOR_DB manquante dans la configuration !");
            _jwtTokenService = jwtTokenService;
            _roleService = roleService;
        }

        /// <summary>
        /// Authentification utilisateur - Retourne un token JWT
        /// </summary>
        /// <param name="dto">Informations de connexion (username/password ou badge)</param>
        /// <returns>Token JWT + informations utilisateur</returns>
        /// <remarks>
        /// ## 🔐 Authentification Dior Enterprise
        /// 
        /// Deux méthodes d'authentification sont supportées :
        /// 
        /// ### 1. Par nom d'utilisateur et mot de passe
        /// ```json
        /// {
        ///   "username": "admin",
        ///   "password": "admin123"
        /// }
        /// ```
        /// 
        /// ### 2. Par badge physique (pour les terminaux)
        /// ```json
        /// {
        ///   "badgePhysicalNumber": "12345"
        /// }
        /// ```
        /// 
        /// ## Comptes de test disponibles :
        /// 
        /// | Rôle | Username | Password | Description |
        /// |------|----------|----------|-------------|
        /// | **Admin** | `admin` | `admin123` | Accès complet système |
        /// | **Manager** | `manager1` | `manager123` | Gestion équipes/projets |
        /// | **RH** | `rh1` | `rh123` | Gestion contrats/paies |
        /// | **Opérateur** | `operateur1` | `operateur123` | Consultation documents |
        /// 
        /// ## Utilisation du token :
        /// 1. Copiez le `token` retourné
        /// 2. Cliquez sur **Authorize** 🔒 en haut
        /// 3. Collez votre token (le préfixe Bearer sera ajouté automatiquement)
        /// 4. Testez les endpoints selon votre rôle
        /// 
        /// **⏰ Durée de validité :** 8 heures
        /// </remarks>
        /// <response code="200">Authentification réussie - Token JWT retourné</response>
        /// <response code="400">Informations d'identification manquantes ou invalides</response>
        /// <response code="401">Identifiants incorrects</response>
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "🔑 Connexion utilisateur",
            Description = "Authentifie un utilisateur et retourne un token JWT valide 8 heures"
        )]
        [SwaggerResponse(200, "Authentification réussie", typeof(LoginResponseDto))]
        [SwaggerResponse(400, "Données invalides")]
        [SwaggerResponse(401, "Identifiants incorrects")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            if (dto is null)
                return BadRequest("Corps de requête manquant.");

            Dior.Library.DTO.UserDto user = null;

            if (!string.IsNullOrEmpty(dto.BadgePhysicalNumber))
            {
                user = GetUserByBadge(dto.BadgePhysicalNumber);
            }
            else if (!string.IsNullOrEmpty(dto.Username) && !string.IsNullOrEmpty(dto.Password))
            {
                user = GetUserByUsernameAndPassword(dto.Username, dto.Password);
            }
            else
            {
                return BadRequest("Informations d'identification manquantes. Fournissez soit username/password soit badgePhysicalNumber.");
            }

            if (user == null)
                return Unauthorized("Identifiants invalides.");

            // Charger rôles via service et extraire les noms
            var roleDefinitions = _roleService.GetRolesByUserId(user.Id);
            user.Roles = roleDefinitions?.Select(r => r.Name).ToList() ?? [];

            var token = _jwtTokenService.GenerateToken(user);
            
            return Ok(new LoginResponseDto 
            { 
                User = user, 
                Token = token,
                ExpiresIn = 28800, // 8 heures en secondes
                TokenType = "Bearer"
            });
        }

        /// <summary>
        /// Informations sur l'utilisateur connecté
        /// </summary>
        /// <returns>Informations de l'utilisateur actuel</returns>
        [HttpGet("me")]
        [SwaggerOperation(
            Summary = "👤 Mon profil",
            Description = "Récupère les informations de l'utilisateur connecté"
        )]
        [SwaggerResponse(200, "Informations utilisateur", typeof(Dior.Library.DTO.UserDto))]
        [SwaggerResponse(401, "Non authentifié")]
        [ProducesResponseType(typeof(Dior.Library.DTO.UserDto), StatusCodes.Status200OK)]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Token invalide");
            }

            // TODO: Récupérer les informations complètes de l'utilisateur
            var userInfo = new
            {
                id = userId,
                username = User.FindFirst(ClaimTypes.Name)?.Value,
                roles = User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList(),
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            };

            return Ok(userInfo);
        }

        /// <summary>
        /// Test de la validité du token JWT
        /// </summary>
        /// <returns>Statut du token</returns>
        [HttpGet("validate")]
        [SwaggerOperation(
            Summary = "✅ Valider token",
            Description = "Vérifie si le token JWT est valide et non expiré"
        )]
        [SwaggerResponse(200, "Token valide")]
        [SwaggerResponse(401, "Token invalide ou expiré")]
        public IActionResult ValidateToken()
        {
            return Ok(new 
            { 
                valid = true, 
                message = "Token JWT valide",
                expiresAt = User.FindFirst("exp")?.Value,
                roles = User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList()
            });
        }

        // Recherche utilisateur par badge
        private Dior.Library.DTO.UserDto GetUserByBadge(string badgePhysicalNumber)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"SELECT u.ID, u.Username, u.LastName, u.FirstName, u.IsActive, u.Email, u.Phone, u.passwordHash FROM [User] u INNER JOIN [UserAccess] ua ON u.ID = ua.UserId INNER JOIN [Access] a ON ua.AccessId = a.ID WHERE a.BadgePhysicalNumber = @badge", conn);
            cmd.Parameters.AddWithValue("@badge", badgePhysicalNumber);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapUser(reader) : null;
        }

        // Recherche utilisateur par login/mdp
        private Dior.Library.DTO.UserDto GetUserByUsernameAndPassword(string username, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM [User] WHERE Username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var hash = reader["passwordHash"] as string;
                if (!string.IsNullOrEmpty(hash) && BCrypt.Net.BCrypt.Verify(password, hash))
                    return MapUser(reader);
            }
            return null;
        }

        private static Dior.Library.DTO.UserDto MapUser(SqlDataReader reader)
        {
            return new Dior.Library.DTO.UserDto
            {
                Id = Convert.ToInt64(reader["ID"]),
                UserName = reader["Username"] as string,
                LastName = reader["LastName"] as string,
                FirstName = reader["FirstName"] as string,
                IsActive = (bool)reader["IsActive"],
                Email = reader["Email"] as string,
                Phone = reader["Phone"] as string,
                Roles = new List<Dior.Library.DTO.RoleDefinitionDto>() // Initialisé vide, sera rempli plus tard
            };
        }
    }

    /// <summary>
    /// Réponse de connexion avec token JWT
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>Informations de l'utilisateur connecté</summary>
        public Dior.Library.DTO.UserDto User { get; set; }
        
        /// <summary>Token JWT pour authentification</summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string Token { get; set; }
        
        /// <summary>Type de token (toujours "Bearer")</summary>
        /// <example>Bearer</example>
        public string TokenType { get; set; } = "Bearer";
        
        /// <summary>Durée de validité en secondes</summary>
        /// <example>28800</example>
        public int ExpiresIn { get; set; }
    }
}

