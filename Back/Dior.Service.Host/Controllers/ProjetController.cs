using Dior.Library.DTO;
using Dior.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authentification requise
    public class ProjetController : ControllerBase
    {
        private readonly IProjetService _projetService;

        public ProjetController(IProjetService projetService)
        {
            _projetService = projetService;
        }

        /// <summary>
        /// GET api/Projet - R�cup�re tous les projets
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ProjetDto>>> GetAll()
        {
            var projets = await _projetService.GetAllAsync();
            return Ok(projets);
        }

        /// <summary>
        /// GET api/Projet/{id} - R�cup�re un projet par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjetDto>> GetById(int id)
        {
            var projet = await _projetService.GetByIdAsync(id);
            return projet == null ? NotFound($"Projet avec l'ID {id} non trouv�") : Ok(projet);
        }

        /// <summary>
        /// GET api/Projet/team/{teamId} - R�cup�re les projets d'une �quipe
        /// </summary>
        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<List<ProjetDto>>> GetByTeamId(int teamId)
        {
            var projets = await _projetService.GetByTeamIdAsync(teamId);
            return Ok(projets);
        }

        /// <summary>
        /// GET api/Projet/my - R�cup�re les projets du manager connect�
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<ProjetDto>>> GetMyProjects()
        {
            var managerId = GetCurrentUserId();
            var projets = await _projetService.GetByManagerIdAsync(managerId);
            return Ok(projets);
        }

        /// <summary>
        /// POST api/Projet - Cr�e un nouveau projet
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<ProjetDto>> Create([FromBody] CreateProjetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // R�cup�rer l'utilisateur depuis le token JWT
            var managerId = GetCurrentUserId();
            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            // Assigner le manager au projet
            request.ManagerId = managerId;

            var projet = await _projetService.CreateAsync(request, userName);
            return CreatedAtAction(nameof(GetById), new { id = projet.Id }, projet);
        }

        /// <summary>
        /// PUT api/Projet/{id} - Met � jour un projet existant
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProjetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // R�cup�rer l'utilisateur depuis le token JWT
            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var result = await _projetService.UpdateAsync(id, request, userName);
            return result ? NoContent() : NotFound($"Projet avec l'ID {id} non trouv�");
        }

        /// <summary>
        /// DELETE api/Projet/{id} - Supprime un projet
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            // V�rifier si c'est le manager du projet ou un admin
            if (!User.IsInRole("Admin"))
            {
                var managerId = GetCurrentUserId();
                var projet = await _projetService.GetByIdAsync(id);
                if (projet == null || projet.ManagerId != managerId)
                    return Forbid("Vous ne pouvez supprimer que vos propres projets");
            }

            var result = await _projetService.DeleteAsync(id);
            return result ? NoContent() : NotFound($"Projet avec l'ID {id} non trouv�");
        }

        /// <summary>
        /// PATCH api/Projet/{id}/progress - Met � jour le progr�s d'un projet
        /// </summary>
        [HttpPatch("{id}/progress")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult> UpdateProgress(int id, [FromBody] int progress)
        {
            if (progress < 0 || progress > 100)
                return BadRequest("Le progr�s doit �tre entre 0 et 100");

            var updateRequest = new UpdateProjetRequest { Progress = progress };
            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            
            var result = await _projetService.UpdateAsync(id, updateRequest, userName);
            return result ? NoContent() : NotFound($"Projet avec l'ID {id} non trouv�");
        }

        private long GetCurrentUserId()
        {
            return long.Parse(User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}