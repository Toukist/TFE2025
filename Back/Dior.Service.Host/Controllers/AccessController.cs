using Dior.Library.DTO.Access;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/access")]
    public class AccessController : ControllerBase
    {
        private readonly AccessService _accessService;
        private readonly ILogger<AccessController> _logger;

        public AccessController(AccessService accessService, ILogger<AccessController> logger)
        {
            _accessService = accessService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AccessDto>>> GetAll()
        {
            var accesses = await _accessService.GetAllAsync();
            return Ok(accesses);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AccessDto>> GetById(int id)
        {
            var access = await _accessService.GetByIdAsync(id);
            if (access == null) return NotFound();
            return Ok(access);
        }

        [HttpPatch("{id:int}/disable")]
        [Authorize(Roles = "Admin,RH")]
        public async Task<IActionResult> DisableBadge(int id)
        {
            var success = await _accessService.SetActiveAsync(id, false);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/enable")]
        [Authorize(Roles = "Admin,RH")]
        public async Task<IActionResult> EnableBadge(int id)
        {
            var success = await _accessService.SetActiveAsync(id, true);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPatch("self-disable")]
        [Authorize]
        public async Task<IActionResult> DisableOwnBadge()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _accessService.DisableUserBadgeAsync(userId);
            return NoContent();
        }
    }
}