using Dior.Library.DTO.Role;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/role-definition")]
    public class RoleDefinitionController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly ILogger<RoleDefinitionController> _logger;

        public RoleDefinitionController(RoleService roleService, ILogger<RoleDefinitionController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDefinitionDto>>> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDefinitionDto>> GetById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleDefinitionDto>> Create([FromBody] CreateRoleDto dto)
        {
            var role = await _roleService.CreateRoleAsync(dto);
            return Ok(role);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            var success = await _roleService.UpdateRoleAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}