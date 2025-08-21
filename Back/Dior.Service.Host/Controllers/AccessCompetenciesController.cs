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
        /// R�cup�re toutes les comp�tences d'acc�s
        /// </summary>
        /// <returns>Liste des comp�tences d'acc�s</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccessCompetencyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccessCompetencyDto>>> GetAll()
        {
            var competencies = await _accessCompetencyService.GetAllAsync();
            return Ok(competencies);
        }

        /// <summary>
        /// R�cup�re une comp�tence d'acc�s par son ID
        /// </summary>
        /// <param name="id">ID de la comp�tence d'acc�s</param>
        /// <returns>Comp�tence d'acc�s trouv�e</returns>
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
        /// R�cup�re les comp�tences d'acc�s enfants
        /// </summary>
        /// <param name="parentId">ID de la comp�tence d'acc�s parent</param>
        /// <returns>Liste des comp�tences d'acc�s enfants</returns>
        [HttpGet("byParent/{parentId}")]
        [ProducesResponseType(typeof(IEnumerable<AccessCompetencyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccessCompetencyDto>>> GetChildren(int parentId)
        {
            var children = await _accessCompetencyService.GetChildrenAsync(parentId);
            return Ok(children);
        }

        /// <summary>
        /// Cr�e une nouvelle comp�tence d'acc�s
        /// </summary>
        /// <param name="createAccessCompetencyDto">Donn�es de la comp�tence d'acc�s</param>
        /// <returns>Nouvelle comp�tence d'acc�s cr��e</returns>
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
        /// Met � jour une comp�tence d'acc�s existante
        /// </summary>
        /// <param name="id">ID de la comp�tence d'acc�s</param>
        /// <param name="updateAccessCompetencyDto">Donn�es � modifier</param>
        /// <returns>R�sultat de la mise � jour</returns>
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
        /// Supprime une comp�tence d'acc�s
        /// </summary>
        /// <param name="id">ID de la comp�tence d'acc�s</param>
        /// <returns>R�sultat de la suppression</returns>
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