using Dior.Library.DTO.Project;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/projet")]
    public class ProjetController : ControllerBase
    {
        private readonly ProjetService _projetService;
        private readonly ILogger<ProjetController> _logger;

        public ProjetController(ProjetService projetService, ILogger<ProjetController> logger)
        {
            _projetService = projetService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjetDto>>> GetAll()
        {
            var projets = await _projetService.GetAllAsync();
            return Ok(projets);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjetDto>> GetById(int id)
        {
            var projet = await _projetService.GetByIdAsync(id);
            if (projet == null) return NotFound();
            return Ok(projet);
        }

        [HttpGet("team/{teamId:int}")]
        public async Task<ActionResult<List<ProjetDto>>> GetByTeamId(int teamId)
        {
            var projets = await _projetService.GetByTeamIdAsync(teamId);
            return Ok(projets);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ProjetDto>> Create([FromBody] CreateProjetDto dto)
        {
            var projet = await _projetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = projet.Id }, projet);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjetDto dto)
        {
            var success = await _projetService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _projetService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}