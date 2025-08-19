using Dior.Database.DTOs.RoleDefinition;
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
    public class RoleDefinitionsController : ControllerBase
    {
        private readonly IRoleDefinitionService _roleDefinitionService;

        public RoleDefinitionsController(IRoleDefinitionService roleDefinitionService)
        {
            _roleDefinitionService = roleDefinitionService;
        }

        /// <summary>
        /// R�cup�re toutes les d�finitions de r�le
        /// </summary>
        /// <returns>Liste des d�finitions de r�le</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDefinitionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDefinitionDto>>> GetAll()
        {
            var roleDefinitions = await _roleDefinitionService.GetAllAsync();
            return Ok(roleDefinitions);
        }

        /// <summary>
        /// R�cup�re une d�finition de r�le par son ID
        /// </summary>
        /// <param name="id">ID de la d�finition de r�le</param>
        /// <returns>D�finition de r�le trouv�e</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDefinitionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDefinitionDto>> Get(int id)
        {
            var roleDefinition = await _roleDefinitionService.GetByIdAsync(id);
            if (roleDefinition == null)
                return NotFound();

            return Ok(roleDefinition);
        }

        /// <summary>
        /// R�cup�re les r�les enfants d'un r�le parent
        /// </summary>
        /// <param name="parentId">ID du r�le parent</param>
        /// <returns>Liste des r�les enfants</returns>
        [HttpGet("byParent/{parentId}")]
        [ProducesResponseType(typeof(IEnumerable<RoleDefinitionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDefinitionDto>>> GetChildRoles(int parentId)
        {
            var childRoles = await _roleDefinitionService.GetChildRolesAsync(parentId);
            return Ok(childRoles);
        }

        /// <summary>
        /// Cr�e une nouvelle d�finition de r�le
        /// </summary>
        /// <param name="createRoleDefinitionDto">Donn�es de la d�finition de r�le</param>
        /// <returns>Nouvelle d�finition de r�le cr��e</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDefinitionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDefinitionDto>> Create([FromBody] CreateRoleDefinitionDto createRoleDefinitionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRoleDefinition = await _roleDefinitionService.CreateAsync(createRoleDefinitionDto);
            return CreatedAtAction(nameof(Get), new { id = createdRoleDefinition.Id }, createdRoleDefinition);
        }

        /// <summary>
        /// Met � jour une d�finition de r�le existante
        /// </summary>
        /// <param name="id">ID de la d�finition de r�le</param>
        /// <param name="updateRoleDefinitionDto">Donn�es � modifier</param>
        /// <returns>R�sultat de la mise � jour</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDefinitionDto updateRoleDefinitionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleDefinitionService.UpdateAsync(id, updateRoleDefinitionDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Supprime une d�finition de r�le
        /// </summary>
        /// <param name="id">ID de la d�finition de r�le</param>
        /// <returns>R�sultat de la suppression</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleDefinitionService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}