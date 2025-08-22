using Dior.Library.DTO.User;
using Dior.Library.DTO.Auth;
using Dior.Service.Host.Services;
using Dior.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IJwtTokenService _jwtTokenService;
        // private readonly IRoleService _roleService; // Temporairement désactivé
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration config,
            IJwtTokenService jwtTokenService,
            // IRoleService roleService, // Temporairement désactivé
            ILogger<AuthController> logger)
        {
            _connectionString = config.GetConnectionString("DIOR_DB") ?? 
                              "Data Source=PC-CORENTIN\\CLEAN22;Initial Catalog=Dior.Database;Integrated Security=True;TrustServerCertificate=True";
            _jwtTokenService = jwtTokenService;
            // _roleService = roleService; // Temporairement désactivé
            _logger = logger;
        }

        /// <summary>
        /// Authentification utilisateur
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                _logger.LogInformation($"🔐 Tentative de connexion pour: {dto?.Username}");

                if (dto == null)
                {
                    _logger.LogWarning("Corps de requête manquant");
                    return BadRequest(new { message = "Corps de requête manquant" });
                }

                // Validation des données
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    return BadRequest(new { message = "Username et Password sont requis" });
                }

                // Authentification
                var user = AuthenticateUser(dto.Username.Trim(), dto.Password);
                
                if (user == null)
                {
                    _logger.LogWarning($"❌ Échec de connexion pour: {dto.Username}");
                    return Unauthorized(new { message = "Identifiants invalides" });
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"⚠️ Compte inactif: {dto.Username}");
                    return Unauthorized(new { message = "Compte désactivé" });
                }

                // Charger les rôles - TEMPORAIREMENT DÉSACTIVÉ
                try
                {
                    // var rolesList = _roleService.GetRolesByUserId(user.Id);
                    // user.Roles = rolesList?.Select(r => r.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>();
                    user.Roles = new List<string> { "User" }; // Rôle par défaut temporaire
                    _logger.LogInformation($"📋 Rôle temporaire assigné pour {user.UserName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du chargement des rôles");
                    user.Roles = new List<string>();
                }

                // Générer le token JWT
                var token = _jwtTokenService.GenerateToken(user);

                _logger.LogInformation($"✅ Connexion réussie pour: {user.UserName} (ID: {user.Id})");

                // Réponse structurée
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    User = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur critique lors de la connexion");
                return StatusCode(500, new { message = "Erreur serveur", detail = ex.Message });
            }
        }

        /// <summary>
        /// Test de santé de l'API Auth
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new 
            { 
                status = "healthy",
                controller = "AuthController",
                timestamp = DateTime.Now,
                database = !string.IsNullOrEmpty(_connectionString)
            });
        }

        /// <summary>
        /// Liste des utilisateurs pour test
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(200)]
        public IActionResult GetUsers()
        {
            try
            {
                var users = new List<object>();
                
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                
                using var cmd = new SqlCommand(@"
                    SELECT TOP 10 
                        ID, 
                        Username, 
                        FirstName, 
                        LastName, 
                        IsActive,
                        Email,
                        CASE 
                            WHEN passwordHash IS NULL THEN 'NO_PASSWORD'
                            WHEN LEFT(passwordHash, 2) = '$2' THEN 'BCRYPT'
                            ELSE 'PLAIN_TEXT'
                        END as PasswordType
                    FROM [USER]
                    ORDER BY ID", conn);
                
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new
                    {
                        id = Convert.ToInt32(reader["ID"]),
                        username = reader["Username"]?.ToString(),
                        name = $"{reader["FirstName"]} {reader["LastName"]}",
                        email = reader["Email"]?.ToString(),
                        isActive = Convert.ToBoolean(reader["IsActive"]),
                        passwordType = reader["PasswordType"]?.ToString()
                    });
                }

                _logger.LogInformation($"📊 {users.Count} utilisateurs récupérés");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return StatusCode(500, new { message = "Erreur", detail = ex.Message });
            }
        }

        private UserDto? AuthenticateUser(string username, string password)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        ID,
                        Username,
                        passwordHash,
                        LastName,
                        FirstName,
                        IsActive,
                        Email,
                        Phone,
                        TeamId
                    FROM [USER] 
                    WHERE Username = @username", conn);

                cmd.Parameters.AddWithValue("@username", username);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogDebug($"Utilisateur non trouvé: {username}");
                    return null;
                }

                var storedHash = reader["passwordHash"]?.ToString();

                if (string.IsNullOrEmpty(storedHash))
                {
                    _logger.LogWarning($"⚠️ Pas de mot de passe pour: {username}");
                    return null;
                }

                // Vérification du mot de passe
                bool passwordValid = false;

                if (storedHash.StartsWith("$2"))
                {
                    // Hash BCrypt
                    try
                    {
                        passwordValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
                        _logger.LogDebug($"Vérification BCrypt: {passwordValid}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erreur BCrypt pour {username}");
                        return null;
                    }
                }
                else
                {
                    // Mot de passe en clair (temporaire)
                    passwordValid = (password == storedHash);
                    if (passwordValid)
                    {
                        _logger.LogWarning($"⚠️ Mot de passe en clair détecté pour {username} - À migrer!");
                    }
                }

                if (!passwordValid)
                {
                    _logger.LogDebug($"Mot de passe incorrect pour: {username}");
                    return null;
                }

                // Créer le DTO - CONVERSION EN INT
                return new UserDto
                {
                    Id = Convert.ToInt32(reader["ID"]),  // Conversion explicite en int
                    UserName = reader["Username"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    Email = reader["Email"]?.ToString() ?? "",
                    Phone = reader["Phone"]?.ToString(),
                    TeamId = reader["TeamId"] == DBNull.Value ? null : Convert.ToInt32(reader["TeamId"])
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'authentification de {username}");
                return null;
            }
        }
    }
}
