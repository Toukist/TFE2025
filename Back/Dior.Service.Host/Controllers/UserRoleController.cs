using Dior.Library.DTO.Role;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/user-role")]
    public class UserRoleController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(UserRoleService userRoleService, ILogger<UserRoleController> logger)
        {
            _userRoleService = userRoleService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserRoleDto>>> GetAll()
        {
            var userRoles = await _userRoleService.GetAllAsync();
            return Ok(userRoles);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<List<UserRoleDto>>> GetByUserId(int userId)
        {
            var userRoles = await _userRoleService.GetByUserIdAsync(userId);
            return Ok(userRoles);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserRoleDto>> AssignRole([FromBody] AssignRoleDto dto)
        {
            var userRole = await _userRoleService.AssignRoleAsync(dto);
            return Ok(userRole);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            var success = await _userRoleService.RemoveRoleAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}