using Dior.Library.DTO.Auth;
using Dior.Library.DTO.User;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    /// <summary>
    /// Contrôleur d'authentification pour le système Dior Enterprise
    /// Support complet : Badge physique + Username/Password
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthenticationService authService,
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        /// <summary>
        /// Authentification utilisateur - Support Badge ET Username/Password
        /// </summary>
        /// <param name="request">Requête de connexion avec badge OU username/password</param>
        /// <returns>Réponse complète avec utilisateur, token et permissions</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseCompleteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Tentative de connexion avec un corps de requête null");
                return BadRequest("Corps de requête manquant");
            }

            if (!request.IsValid())
            {
                _logger.LogWarning("Tentative de connexion avec des paramètres invalides");
                return BadRequest("Username/Password ou BadgePhysicalNumber requis");
            }

            try
            {
                UserDto? user = null;

                // Authentification par badge physique
                if (!string.IsNullOrEmpty(request.BadgePhysicalNumber))
                {
                    _logger.LogInformation("Tentative d'authentification par badge: {Badge}", 
                        request.BadgePhysicalNumber);
                    user = await _authService.AuthenticateByBadgeAsync(request.BadgePhysicalNumber);
                }
                // Authentification par username/password
                else if (!string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password))
                {
                    _logger.LogInformation("Tentative d'authentification par credentials: {Username}", 
                        request.Username);
                    user = await _authService.AuthenticateByCredentialsAsync(request.Username, request.Password);
                }

                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Échec d'authentification - Identifiants invalides ou compte désactivé");
                    return Unauthorized("Identifiants invalides ou compte désactivé");
                }

                // Charger les informations complètes de l'utilisateur
                await LoadUserCompleteInfoAsync(user);

                // Générer le token JWT
                var token = _jwtTokenService.GenerateToken(user);
                var expiresAt = DateTime.UtcNow.AddMinutes(60); // Configurable

                var response = new LoginResponseCompleteDto
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    UserId = user.Id,
                    UserName = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    TeamId = user.TeamId,
                    TeamName = user.TeamName,
                    Roles = user.Roles?.Select(r => r.Name).ToList() ?? new List<string>(),
                    AccessCompetencies = user.AccessCompetencies ?? new List<string>(),
                    IsActive = user.IsActive,
                    Phone = user.Phone,
                    BadgePhysicalNumber = user.BadgePhysicalNumber
                };

                _logger.LogInformation("Connexion réussie pour l'utilisateur: {Username} (ID: {UserId})", 
                    user.Username, user.Id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la tentative de connexion");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Validation d'un token JWT
        /// </summary>
        /// <returns>Informations du token si valide</returns>
        [HttpGet("validate")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                var accessCompetencies = User.FindAll("access_competency").Select(c => c.Value).ToList();

                var response = new
                {
                    valid = true,
                    message = "Token JWT valide",
                    userId = userId,
                    username = username,
                    roles = roles,
                    accessCompetencies = accessCompetencies,
                    expiresAt = User.FindFirst("exp")?.Value
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la validation du token");
                return Unauthorized();
            }
        }

        /// <summary>
        /// Déconnexion utilisateur (côté client principalement)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                _logger.LogInformation("Déconnexion de l'utilisateur: {Username}", username);

                return Ok(new { message = "Déconnexion réussie" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la déconnexion");
                return Ok(new { message = "Déconnexion effectuée" });
            }
        }

        /// <summary>
        /// Changement de mot de passe pour l'utilisateur connecté
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Token invalide");
                }

                var success = await _authService.ChangePasswordAsync(
                    userId, request.CurrentPassword, request.NewPassword);

                if (!success)
                {
                    return BadRequest("Mot de passe actuel incorrect");
                }

                _logger.LogInformation("Mot de passe changé avec succès pour l'utilisateur: {UserId}", userId);
                return Ok(new { message = "Mot de passe changé avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les informations de l'utilisateur connecté
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Token invalide");
                }

                // Pour cette version, on reconstruit l'utilisateur depuis les claims
                var user = new UserDto
                {
                    Id = userId,
                    Username = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                    FirstName = User.FindFirst("firstName")?.Value ?? "",
                    LastName = User.FindFirst("lastName")?.Value ?? "",
                    Email = User.FindFirst("email")?.Value ?? "",
                    Phone = User.FindFirst("phone")?.Value,
                    IsActive = bool.Parse(User.FindFirst("isActive")?.Value ?? "true"),
                    TeamId = int.TryParse(User.FindFirst("teamId")?.Value, out var teamId) ? teamId : null,
                    TeamName = User.FindFirst("teamName")?.Value,
                    BadgePhysicalNumber = User.FindFirst("badgeNumber")?.Value,
                    Roles = User.FindAll(ClaimTypes.Role).Select(r => new RoleDefinitionDto 
                    { 
                        Name = r.Value, 
                        IsActive = true 
                    }).ToList(),
                    AccessCompetencies = User.FindAll("access_competency").Select(c => c.Value).ToList()
                };

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des informations utilisateur");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Méthode privée pour charger les informations complètes d'un utilisateur
        /// </summary>
        private async Task LoadUserCompleteInfoAsync(UserDto user)
        {
            // Charger les rôles
            user.Roles = await _authService.GetUserRolesAsync(user.Id);

            // Charger les compétences d'accès
            user.AccessCompetencies = await _authService.GetUserAccessCompetenciesAsync(user.Id);

            // Charger le nom de l'équipe
            if (user.TeamId.HasValue)
            {
                user.TeamName = await _authService.GetUserTeamNameAsync(user.TeamId.Value);
            }
        }
    }
}