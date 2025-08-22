using Dior.Library.DTO.Auth;
using Dior.Library.DTO.User;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // IMPORTANT: Microsoft.Data, pas System.Data
using BCrypt.Net;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration config,
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _connectionString = config.GetConnectionString("DIOR_DB");
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                _logger.LogInformation($"Tentative de connexion pour: {dto?.Username}");

                if (dto == null)
                    return BadRequest(new { message = "Corps de requête manquant" });

                if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                    return BadRequest(new { message = "Username et Password requis" });

                var user = AuthenticateUser(dto.Username, dto.Password);
                
                if (user == null)
                {
                    _logger.LogWarning($"Échec de connexion pour: {dto.Username}");
                    return Unauthorized(new { message = "Authentification échouée" });
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"Compte inactif: {dto.Username}");
                    return Unauthorized(new { message = "Compte désactivé" });
                }

                // Générer le token JWT
                var token = _jwtTokenService.GenerateToken(user);

                _logger.LogInformation($"Connexion réussie pour: {dto.Username}");

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    User = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion");
                return StatusCode(500, new { message = "Erreur serveur", detail = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { 
                message = "API Auth fonctionnelle",
                timestamp = DateTime.Now,
                connectionString = !string.IsNullOrEmpty(_connectionString)
            });
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = new List<object>();
                
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                
                using var cmd = new SqlCommand(
                    "SELECT TOP 10 ID, Username, FirstName, LastName, IsActive FROM [USER]", 
                    conn);
                
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new
                    {
                        id = reader.GetInt64(0),
                        username = reader.GetString(1),
                        firstName = reader.GetString(2),
                        lastName = reader.GetString(3),
                        isActive = reader.GetBoolean(4)
                    });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return StatusCode(500, new { message = "Erreur", detail = ex.Message });
            }
        }

        private UserDto AuthenticateUser(string username, string password)
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
                    _logger.LogWarning($"Utilisateur non trouvé: {username}");
                    return null;
                }

                var storedHash = reader["passwordHash"]?.ToString();

                if (string.IsNullOrEmpty(storedHash))
                {
                    _logger.LogWarning($"Pas de mot de passe pour: {username}");
                    return null;
                }

                // Vérifier le mot de passe
                bool passwordValid = false;

                if (storedHash.StartsWith("$2"))
                {
                    // Hash BCrypt
                    try
                    {
                        passwordValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
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
                    _logger.LogWarning($"Mot de passe en clair détecté pour {username}");
                }

                if (!passwordValid)
                {
                    _logger.LogWarning($"Mot de passe incorrect pour: {username}");
                    return null;
                }

                // Créer le DTO - IMPORTANT: Utiliser int pour l'ID
                return new UserDto
                {
                    Id = Convert.ToInt32(reader["ID"]), // Convertir en int
                    UserName = reader["Username"]?.ToString(),
                    LastName = reader["LastName"]?.ToString(),
                    FirstName = reader["FirstName"]?.ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    Email = reader["Email"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    TeamId = reader["TeamId"] as int?
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
