using Dior.Library.DTO.User;
using Dior.Library.DTO.Access;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers.Users
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère tous les utilisateurs
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        /// <summary>
        /// Récupère un utilisateur par ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound($"Utilisateur avec l'ID {id} non trouvé");
            
            return Ok(user);
        }

        /// <summary>
        /// Récupère l'utilisateur connecté
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("userId")?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Crée un nouvel utilisateur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,RH")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdUser = await _userService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'utilisateur");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,RH")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _userService.ExistsAsync(id);
            if (!exists)
                return NotFound($"Utilisateur avec l'ID {id} non trouvé");

            try
            {
                await _userService.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur {Id}", id);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _userService.ExistsAsync(id);
            if (!exists)
                return NotFound($"Utilisateur avec l'ID {id} non trouvé");

            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Récupère les utilisateurs avec leurs détails complets
        /// </summary>
        [HttpGet("full")]
        [Authorize(Roles = "Admin,RH,Manager")]
        [ProducesResponseType(typeof(List<UserFullDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserFullDto>>> GetFullUsers()
        {
            var users = await _userService.GetFullUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Récupère les rôles d'un utilisateur
        /// </summary>
        [HttpGet("{id:int}/roles")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<string>>> GetUserRoles(int id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }

        /// <summary>
        /// Récupère les compétences d'accès d'un utilisateur
        /// </summary>
        [HttpGet("{id:int}/access-competencies")]
        [ProducesResponseType(typeof(List<AccessCompetencyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AccessCompetencyDto>>> GetUserAccessCompetencies(int id)
        {
            var competencies = await _userService.GetUserAccessCompetenciesAsync(id);
            return Ok(competencies);
        }
    }
}