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
        /// Récupère toutes les définitions de rôle
        /// </summary>
        /// <returns>Liste des définitions de rôle</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDefinitionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDefinitionDto>>> GetAll()
        {
            var roleDefinitions = await _roleDefinitionService.GetAllAsync();
            return Ok(roleDefinitions);
        }

        /// <summary>
        /// Récupère une définition de rôle par son ID
        /// </summary>
        /// <param name="id">ID de la définition de rôle</param>
        /// <returns>Définition de rôle trouvée</returns>
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
        /// Récupère les rôles enfants d'un rôle parent
        /// </summary>
        /// <param name="parentId">ID du rôle parent</param>
        /// <returns>Liste des rôles enfants</returns>
        [HttpGet("byParent/{parentId}")]
        [ProducesResponseType(typeof(IEnumerable<RoleDefinitionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDefinitionDto>>> GetChildRoles(int parentId)
        {
            var childRoles = await _roleDefinitionService.GetChildRolesAsync(parentId);
            return Ok(childRoles);
        }

        /// <summary>
        /// Crée une nouvelle définition de rôle
        /// </summary>
        /// <param name="createRoleDefinitionDto">Données de la définition de rôle</param>
        /// <returns>Nouvelle définition de rôle créée</returns>
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
        /// Met à jour une définition de rôle existante
        /// </summary>
        /// <param name="id">ID de la définition de rôle</param>
        /// <param name="updateRoleDefinitionDto">Données à modifier</param>
        /// <returns>Résultat de la mise à jour</returns>
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
        /// Supprime une définition de rôle
        /// </summary>
        /// <param name="id">ID de la définition de rôle</param>
        /// <returns>Résultat de la suppression</returns>
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