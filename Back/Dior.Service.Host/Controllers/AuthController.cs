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
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Authentification utilisateur - Retourne un token JWT
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseCompleteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                // Version simplifiée pour tester
                if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                {
                    return Task.FromResult<IActionResult>(BadRequest("Username et Password requis"));
                }

                // Test simple avec admin/admin
                if (dto.Username == "admin" && dto.Password == "admin")
                {
                    var user = new UserDto
                    {
                        Id = 1,
                        UserName = "admin",
                        FirstName = "Admin",
                        LastName = "System",
                        Email = "admin@dior.com",
                        IsActive = true,
                        Roles = new List<string> { "Admin" }
                    };

                    var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.UserName, user.Roles);

                    var response = new LoginResponseCompleteDto
                    {
                        Token = token,
                        ExpiresAt = DateTime.UtcNow.AddHours(8),
                        UserId = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        TeamId = user.TeamId,
                        TeamName = user.TeamName,
                        Roles = user.Roles
                    };

                    return Task.FromResult<IActionResult>(Ok(response));
                }

                return Task.FromResult<IActionResult>(Unauthorized("Authentification échouée"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(500, $"Erreur serveur: {ex.Message}"));
            }
        }

        /// <summary>
        /// Valider token JWT
        /// </summary>
        [HttpGet("validate")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    }
}