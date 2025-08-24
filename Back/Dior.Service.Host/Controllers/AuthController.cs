using Dior.Library.DTO.Auth;
using Dior.Library.DTO.User;
using Dior.Service.Host.Services;
using Dior.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly DiorDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly ILogger<AuthController> _logger;

        public AuthController(DiorDbContext db, IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _passwordHasher = new PasswordHasher<string>();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseCompleteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Username et Password requis");

            var user = await _db.Users
                .Where(u => u.UserName == dto.Username)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.TeamId,
                    TeamName = u.Team != null ? u.Team.Name : null,
                    u.IsActive,
                    u.PasswordHash,
                    Roles = u.UserRoles.Select(ur => ur.RoleDefinition.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("�chec login pour {User} : inexistant/inactif", dto.Username);
                return Unauthorized("Authentification �chou�e");
            }

            // V�rification du mot de passe hash�
            var result = _passwordHasher.VerifyHashedPassword(user.UserName, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("�chec login pour {User} : mot de passe incorrect", dto.Username);
                return Unauthorized("Authentification �chou�e");
            }

            // G�n�rer le JWT
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

            return Ok(response);
        }

        /// <summary>
        /// Valider un token JWT
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
