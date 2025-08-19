using Dior.Database.DTOs.AccessCompetency;
using Dior.Database.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccessCompetenciesController : ControllerBase
    {
        private readonly IAccessCompetencyService _accessCompetencyService;

        public AccessCompetenciesController(IAccessCompetencyService accessCompetencyService)
        {
            _accessCompetencyService = accessCompetencyService;
        }

        /// <summary>
        /// Récupère toutes les compétences d'accès
        /// </summary>
        /// <returns>Liste des compétences d'accès</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccessCompetencyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccessCompetencyDto>>> GetAll()
        {
            var competencies = await _accessCompetencyService.GetAllAsync();
            return Ok(competencies);
        }

        /// <summary>
        /// Récupère une compétence d'accès par son ID
        /// </summary>
        /// <param name="id">ID de la compétence d'accès</param>
        /// <returns>Compétence d'accès trouvée</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccessCompetencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccessCompetencyDto>> Get(int id)
        {
            var competency = await _accessCompetencyService.GetByIdAsync(id);
            if (competency == null)
                return NotFound();

            return Ok(competency);
        }

        /// <summary>
        /// Récupère les compétences d'accès enfants
        /// </summary>
        /// <param name="parentId">ID de la compétence d'accès parent</param>
        /// <returns>Liste des compétences d'accès enfants</returns>
        [HttpGet("byParent/{parentId}")]
        [ProducesResponseType(typeof(IEnumerable<AccessCompetencyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccessCompetencyDto>>> GetChildren(int parentId)
        {
            var children = await _accessCompetencyService.GetChildrenAsync(parentId);
            return Ok(children);
        }

        /// <summary>
        /// Crée une nouvelle compétence d'accès
        /// </summary>
        /// <param name="createAccessCompetencyDto">Données de la compétence d'accès</param>
        /// <returns>Nouvelle compétence d'accès créée</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AccessCompetencyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccessCompetencyDto>> Create([FromBody] CreateAccessCompetencyDto createAccessCompetencyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCompetency = await _accessCompetencyService.CreateAsync(createAccessCompetencyDto);
            return CreatedAtAction(nameof(Get), new { id = createdCompetency.Id }, createdCompetency);
        }

        /// <summary>
        /// Met à jour une compétence d'accès existante
        /// </summary>
        /// <param name="id">ID de la compétence d'accès</param>
        /// <param name="updateAccessCompetencyDto">Données à modifier</param>
        /// <returns>Résultat de la mise à jour</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAccessCompetencyDto updateAccessCompetencyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accessCompetencyService.UpdateAsync(id, updateAccessCompetencyDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Supprime une compétence d'accès
        /// </summary>
        /// <param name="id">ID de la compétence d'accès</param>
        /// <returns>Résultat de la suppression</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accessCompetencyService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}